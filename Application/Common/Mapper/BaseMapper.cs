using AppCommon.DTOs;
using AppCommon.GlobalHelpers;

namespace Application.Common.Mapper
{
    public abstract class BaseMapper
    {
        protected DateTimeParseResult MapDateTime(DateTime source)
            => source.FormatDateTime();

        protected DateTimeParseResult MapDateTime(DateTime? source)
            => source.FormatDateTime();
    }
}
