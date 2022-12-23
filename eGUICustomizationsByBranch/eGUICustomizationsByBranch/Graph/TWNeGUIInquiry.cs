using PX.SM;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using eGUICustomizations.DAC;

namespace eGUICustomizations.Graph
{
    public class TWNeGUIInquiry : PXGraph<TWNeGUIInquiry>
    {
        #region Select & Features
        public PXSavePerRow<TWNGUITrans> Save;
        public PXCancel<TWNGUITrans> Cancel;

        [PXFilterable]
        [PXImport(typeof(TWNGUITrans))]
        public SelectFrom<TWNGUITrans>.View ViewGUITrans;
        #endregion

        #region Event Handlers
        protected void _(Events.RowSelected<TWNGUITrans> e)
        {
            bool hasExported = e.Row?.EGUIExported ?? false;

            PXUIFieldAttribute.SetEnabled<TWNGUITrans.gUINbr>      (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.batchNbr>    (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.orderNbr>    (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.qREncrypter> (e.Cache, e.Row, false);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.taxNbr>      (e.Cache, e.Row, !hasExported);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.ourTaxNbr>   (e.Cache, e.Row, !hasExported);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.gUITitle>    (e.Cache, e.Row, !hasExported);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.addressLine> (e.Cache, e.Row, !hasExported);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.netAmount>   (e.Cache, e.Row, !hasExported);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.netAmtRemain>(e.Cache, e.Row, !hasExported);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.taxAmount>   (e.Cache, e.Row, !hasExported);
            PXUIFieldAttribute.SetEnabled<TWNGUITrans.taxAmtRemain>(e.Cache, e.Row, !hasExported);

            PXImportAttribute.SetEnabled(this, ViewGUITrans.Name, AssignedRole("Financial Supervisor"));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Check whether the access user is assigned role parameters.
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        protected bool AssignedRole(params string[] roleName) => SelectFrom<UsersInRoles>.Where<UsersInRoles.rolename.IsEqual<@P.AsString>
                                                                                         .And<UsersInRoles.username.IsEqual<AccessInfo.userName.FromCurrent>>>
                                                                                         .View.ReadOnly.Select(this, roleName) != null;
        #endregion
    }
}