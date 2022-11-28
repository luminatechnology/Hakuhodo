using PX.Data;

namespace UCG_Customization.Descriptor
{
    public class UserDefinedFieldAttribute : PXEventSubscriberAttribute, IPXRowUpdatedSubscriber, IPXRowInsertedSubscriber
    {
        protected string _attributeID;
        public UserDefinedFieldAttribute(string attributeID)
        {
            _attributeID = PX.Objects.CS.Messages.Attribute + attributeID;
        }

        public override void CacheAttached(PXCache sender) {
            base.CacheAttached(sender);
            sender.Graph.RowSelecting.AddHandler(sender.GetItemType(), delegate (PXCache cache, PXRowSelectingEventArgs e)
            {
                setUDFValue(cache, e.Row);
            });
        }

        public void RowInserted(PXCache sender, PXRowInsertedEventArgs e)
        {
            setUDFValue(sender, e.Row);
        }

        public void RowUpdated(PXCache sender, PXRowUpdatedEventArgs e)
        {
            setUDFValue(sender, e.Row);
        }

        private void setUDFValue(PXCache sender, object item)
        {
            if (item == null) return;
            PXFieldState state = sender.GetValue(item, _attributeID) as PXFieldState;
            sender.SetValue(item, _FieldName, state?.Value);
        }
    }
}
