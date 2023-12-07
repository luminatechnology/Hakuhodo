using PX.Data;
using PX.Data.WorkflowAPI;

namespace PX.Objects.GL
{
    public class JournalEntryUCGExt : PXGraphExtension<JournalEntry_ApprovalWorkflow, JournalEntry>
    {
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
