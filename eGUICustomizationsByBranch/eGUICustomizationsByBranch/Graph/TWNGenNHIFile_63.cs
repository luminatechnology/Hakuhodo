using PX.Data;

namespace eGUICustomizationsByBranch.Graph
{
    public class TWNGenNHIFile_63 : TWNGenerateNHIFileBase
    {
        public TWNGenNHIFile_63()
        {
            base.HeaderMsg = "*資料識別碼,統一編號,所得類別,給付起始年月,給付結束年月,申報總筆數,所得(收入)給付總額,扣繳補充保險費總額,扣費義務人,聯絡電話,電子郵件信箱,聯絡人姓名";
            base.DetailMsg = "*資料識別碼,處理方式(新增I  覆蓋R),給付日期,所得人身分證號,所得人姓名,單次給付金額,扣繳補充保險費金額,申報編號(詳格式說明),信託註記,資料註記,,";
            base.NHIGenType = "63";
        }

        protected virtual void _(Events.FieldDefaulting<TWNGenerateNHIFileFilter.secNHICode> e)
        {   
            e.NewValue = "50";
        }
    }
}