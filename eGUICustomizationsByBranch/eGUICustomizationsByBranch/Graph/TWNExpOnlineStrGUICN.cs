using PX.Data;
using eGUICustomizations.DAC;
using eGUICustomizations.Descriptor;

namespace eGUICustomizations.Graph
{
    public class TWNExpOnlineStrGUICN : PXGraph<TWNExpOnlineStrGUICN>
    {
        #region Features
        public PXCancel<WHTTranFilter> Cancel;
        public PXFilter<WHTTranFilter> Filter;
        public PXFilteredProcessing<TWNGUITrans,
                                    WHTTranFilter,
                                    Where<TWNGUITrans.eGUIExcluded, NotEqual<True>,
                                          And<TWNGUITrans.gUIFormatCode, Equal<PX.Objects.AR.ARRegisterExt.VATOut33Att>,
                                               And2<Where<TWNGUITrans.eGUIExported, Equal<False>,
                                                          Or<TWNGUITrans.eGUIExported, IsNull>>,
                                                    And<TWNGUITrans.branchID, Equal<Current<WHTTranFilter.branchID>>>>>>> GUITranProc;
        #endregion

        #region Ctor
        public TWNExpOnlineStrGUICN()
        {
            GUITranProc.SetProcessCaption(ActionsMessages.Upload);
            GUITranProc.SetProcessAllCaption(TWMessages.UploadAll);
            GUITranProc.SetProcessDelegate(TWNExpOnlineStrGUIInv.Upload);
        }
        #endregion 
    }
}