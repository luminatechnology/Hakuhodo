using PX.Data;

namespace UCG_Customization.Utils
{
    public class ErrorMsg
    {
        public static void SetError<Field>(PXCache cache, object row, object newValue, string errorMsg, PXErrorLevel errorLevel = PXErrorLevel.Error) where Field : PX.Data.IBqlField
        {
            cache.RaiseExceptionHandling<Field>(row, newValue,
                  new PXSetPropertyException(errorMsg, errorLevel));
        }

        public static void SetError(PXCache cache,string fieldName ,object row, object newValue, string errorMsg, PXErrorLevel errorLevel = PXErrorLevel.Error)
        {
            cache.RaiseExceptionHandling(fieldName, row, newValue,
                  new PXSetPropertyException(errorMsg, errorLevel));
        }
    }
}
