using PX.Data;
using PX.Objects.EP;
using PX.Objects.PM;
using PX.TM;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UCG_Customization.Utils
{
    public class ApproveWGUtil
    {
        #region const
        const string USR_APPROVE_WG = "UsrApproveWG{0:00}";
        const int TREE_SIZE = 10;
        /**User Defined */
        const string UD_APPROVE1 = "AttributeAPPROVER1";
        /**User Defined */
        const string UD_APPROVE2 = "AttributeAPPROVER2";
        /**User Defined */
        const string UD_APPROVE3 = "AttributeAPPROVER3";
        /**User Defined */
        const string UD_APPROVE4 = "AttributeAPPROVER4";
        /**User Defined */
        const string UD_APPROVE5 = "AttributeAPPROVER5";

        /// <summary> 無簽核層級Project:INT001 </summary>
        const string NO_APPROVE_PROJECT_INT001 = "INT001";
        /// <summary> 無簽核層級Project:INT002 </summary>
        const string NO_APPROVE_PROJECT_INT002 = "INT002";
        #endregion


        public static void SetUsrApproveWG(PXCache cache, object row, string deptID, string empAcctCD, int? projectID = null)
        {
            PXGraph graph = new PXGraph();
            #region 取得層級資料
            string[] saveAcctCDs = new string[TREE_SIZE];
            PMProject project = PMProject.PK.Find(cache.Graph, projectID);
            string projectCD = project?.ContractCD.Trim();
            if (projectID == null
                || projectID == ProjectDefaultAttribute.NonProject()
                || IsNoApproveProject(projectCD))
            {
                #region 一般情境
                //取得當前階層
                EPCompanyTree thisTree = GetCompanyTreeByDeptID(graph, deptID);
                if (thisTree != null)
                {
                    saveAcctCDs = GetUsrApproveWG(graph, thisTree);
                }
                #endregion
            }
            else
            {
                #region 需判斷Project 且不為NonProject時
                saveAcctCDs = GetUsrApproveWGByNonProject(graph, projectID);
                #endregion
            }
            #endregion

            string preAcctCD = null;
            bool isSelf = false;
            //寫入欄位
            for (int i = 1; i <= TREE_SIZE; i++)
            {
                //當前簽核者
                var thisAcctCD = saveAcctCDs[i - 1];
                //當前簽核者為經辦人時...就代表是自己
                if (thisAcctCD == empAcctCD) isSelf = true;
                //當曾經為自己時，當前的acctCD 要拿上一層的
                //如果當前的acctCD為空，也是拿上一層(94要填滿
                if (isSelf || thisAcctCD == null)
                {
                    thisAcctCD = preAcctCD ?? thisAcctCD;//如果為最上層人員....preAcctCD會為null，那就先帶自己[----這段之後可能會改----]
                }
                cache.SetValueExt(row, string.Format(USR_APPROVE_WG, i), thisAcctCD);
                preAcctCD = thisAcctCD;
            }
        }

        private static string[] GetUsrApproveWGByNonProject(PXGraph graph, int? projectID)
        {
            //建立虛構空間
            string[] saveAcctCDs = new string[TREE_SIZE];
            PMProject project = PMProject.PK.Find(graph, projectID);
            PXCache<PMProject> projectCache = graph.Caches<PMProject>();
            saveAcctCDs[4] = (PXStringState)projectCache.GetValueExt(project, UD_APPROVE5);
            saveAcctCDs[5] = (PXStringState)projectCache.GetValueExt(project, UD_APPROVE4);
            saveAcctCDs[6] = (PXStringState)projectCache.GetValueExt(project, UD_APPROVE3);
            saveAcctCDs[7] = (PXStringState)projectCache.GetValueExt(project, UD_APPROVE2);
            saveAcctCDs[8] = (PXStringState)projectCache.GetValueExt(project, UD_APPROVE1);
            return saveAcctCDs;
        }

        private static string[] GetUsrApproveWG(PXGraph graph, EPCompanyTree thisTree)
        {
            //建立虛構空間
            string[] saveAcctCDs = new string[TREE_SIZE];
            List<int?> tempAcctCDs = new List<int?>();
            tempAcctCDs.Add(thisTree.WorkGroupID);

            LoopGetCompanyTreeByParentID(graph, tempAcctCDs, thisTree?.ParentWGID);

            if (tempAcctCDs.Count > TREE_SIZE)
            {
                string ERR_MSG = "公司組織結構超過" + TREE_SIZE + "層，請聯絡系統管理員維護程式";
                throw new PXException(ERR_MSG);
            }

            //tempAcctCDs 倒續，同時取得對應的employeeCD
            int j = 0;
            for (int i = tempAcctCDs.Count - 1; i >= 0; i--)
            {
                saveAcctCDs[j++] = GetTreeMasterEmpCD(graph, tempAcctCDs[i])?.Trim();
            }
            return saveAcctCDs;
        }


        /***
         * 迴圈Method --注意使用避免無窮迴圈!!!
         */
        private static void LoopGetCompanyTreeByParentID(PXGraph graph, List<int?> tempIDs, int? parentID)
        {
            //ParentID 為0 代表此為最上層
            if (parentID == null || parentID == 0) return;
            EPCompanyTree parentTree = GetParentCompanyTree(graph, parentID);
            if (parentTree == null) return;
            tempIDs.Add(parentTree.WorkGroupID);

            LoopGetCompanyTreeByParentID(graph, tempIDs, parentTree.ParentWGID);
        }

        /// <summary>
        /// 判斷是否是要忽略的Project
        /// </summary>
        private static bool IsNoApproveProject(string projectCD)
        {
            string[] noApproveProject = { NO_APPROVE_PROJECT_INT001, NO_APPROVE_PROJECT_INT002 };
            foreach (var cd in noApproveProject)
            {
                //start with NoApproveProject
                string pattern = $"^({cd})[A-Za-z0-9]*$";
                if (Regex.IsMatch(projectCD, pattern)) return true;
            }
            return false;
        }

        #region BQL
        private static EPCompanyTree GetCompanyTreeByDeptID(PXGraph graph, string deptID)
        {
            return PXSelectJoin<EPCompanyTree,
                InnerJoin<EPDepartment, On<EPCompanyTree.workGroupID, Equal<EPDepartmentExt.usrWorkGroupID>>>,
                Where<EPDepartment.departmentID, Equal<Required<EPDepartment.departmentID>>>>.Select(graph, deptID);
        }

        private static EPCompanyTree GetParentCompanyTree(PXGraph graph, int? thisWorkGroupID)
        {
            return PXSelect<EPCompanyTree,
                Where<EPCompanyTree.workGroupID, Equal<Required<EPCompanyTree.workGroupID>>>>.Select(graph, thisWorkGroupID);
        }

        private static string GetTreeMasterEmpCD(PXGraph graph, int? workGroupID)
        {
            EPEmployee emp = PXSelectJoin<EPEmployee,
                InnerJoin<EPCompanyTreeMember, On<EPCompanyTreeMember.contactID, Equal<EPEmployee.defContactID>>>,
                Where<EPCompanyTreeMember.workGroupID, Equal<Required<EPCompanyTreeMember.workGroupID>>,
                And<EPCompanyTreeMember.active, Equal<True>>>,
                OrderBy<
                 Desc<EPCompanyTreeMember.isOwner>
                >>.Select(graph, workGroupID);
            return emp?.AcctCD;
        }


        #endregion
    }
}
