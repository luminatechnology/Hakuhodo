using PX.Data;
using PX.Data.WorkflowAPI;
using System.Collections;

namespace PX.Objects.GL
{
    public class JournalEntryUCGExt : PXGraphExtension<JournalEntry_ApprovalWorkflow, JournalEntry>
    {
        #region Override Action
        public delegate IEnumerable ReleaseFromHoldDelegate(PXAdapter adapter);
        [PXOverride]
        public virtual IEnumerable ReleaseFromHold(PXAdapter adapter)
        {
            var batch = Base.BatchModule.Current;
            if (batch.CuryDebitTotal != batch.CuryCreditTotal)
            {
                string fieldName = batch.CuryDebitTotal > batch.CuryCreditTotal ? typeof(Batch.curyDebitTotal).Name : typeof(Batch.curyCreditTotal).Name;
                object value = batch.CuryDebitTotal > batch.CuryCreditTotal ? batch.CuryDebitTotal : batch.CuryCreditTotal;
                Base.BatchModule.Cache.RaiseExceptionHandling(fieldName, batch, value,
                    new PXSetPropertyException(Messages.BatchOutOfBalance)
                    );
                throw new PXException(Messages.BatchOutOfBalance);
            }
            //return baseMethod(adapter);
            return adapter.Get();
        }
        #endregion

        #region Override Workflow
        public override void Configure(PXScreenConfiguration config)
        {
            Configure(config.GetScreenConfigurationContext<JournalEntry, Batch>());
        }

        protected virtual void Configure(WorkflowContext<JournalEntry, Batch> context)
        {
            context.UpdateScreenConfigurationFor(screen =>
                screen.UpdateDefaultFlow(flow =>
                    flow.WithFlowStates(fss =>
                        {
                            fss.Update<BatchStatus.hold>(flowState =>
                            {
                                return flowState.WithActions(actions =>
                                       {
                                           actions.Add(g => g.glEditDetails);
                                       });
                            });
                        })
                    )
                );
        }
        #endregion
    }
}
