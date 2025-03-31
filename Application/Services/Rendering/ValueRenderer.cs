using AppCommon.DTOs;
using AppCommon.EnumShared;
using AppCommon.GlobalHelpers;
using Module.Domain.Data;
using System.Globalization;

namespace Application.Services.Rendering
{
    public class TextValueRenderer : IValueRenderer
    {
        public async Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken cancellationToken = default)
        {
            if (propertyData?.Property == null)
                return null;

            var viewType = propertyData.Property.ViewType;
            if (viewType != ViewTypeEnum.Text
                && viewType != ViewTypeEnum.LongText
                && viewType != ViewTypeEnum.RichText)
            {
                return null;
            }

            var textValue = new TextValue();
            bool isTranslatable = propertyData.Property.IsTranslatable;

            if (isTranslatable)
            {
                if (!string.IsNullOrEmpty(propertyData.StringValue))
                {
                    try
                    {
                        var translatable = propertyData.StringValue.FromJson<TranslatableValue>();
                        textValue.Translatable = translatable;
                        textValue.DisplayValue = translatable.GetLocalizedValue();
                    }
                    catch
                    {
                        textValue.Translatable = new TranslatableValue { En = propertyData.StringValue };
                        textValue.DisplayValue = textValue.Translatable.GetLocalizedValue();
                    }
                }
                else
                {
                    textValue.Translatable = new TranslatableValue();
                    textValue.DisplayValue = string.Empty;
                }
            }
            else
            {
                textValue.Translatable = null;
                textValue.DisplayValue = propertyData.StringValue ?? string.Empty;
            }
            return await Task.FromResult(textValue);
        }
    }

    public class NumericValueRenderer : IValueRenderer
    {
        public Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken ct)
        {
            decimal numericVal = 0;
            if (propertyData.DataType == DataTypeEnum.Int && propertyData.IntValue.HasValue)
                numericVal = propertyData.IntValue.Value;
            else if (propertyData.DataType == DataTypeEnum.Double && propertyData.DoubleValue.HasValue)
                numericVal = (decimal)propertyData.DoubleValue.Value;
            else if (propertyData.DataType == DataTypeEnum.Decimal && propertyData.DecimalValue.HasValue)
                numericVal = propertyData.DecimalValue.Value;
            else if (propertyData.DataType == DataTypeEnum.Bool && propertyData.BoolValue.HasValue)
                numericVal = propertyData.BoolValue.Value ? 1 : 0;

            var formatted = numericVal.ToString(CultureInfo.CurrentCulture);
            switch (propertyData.Property.ViewType)
            {
                case ViewTypeEnum.Currency:
                    formatted = numericVal.ToString("C", CultureInfo.CurrentCulture);
                    break;
                case ViewTypeEnum.Percentage:
                    formatted = numericVal.ToString("P", CultureInfo.CurrentCulture);
                    break;
                case ViewTypeEnum.Float:
                    formatted = numericVal.ToString("F2", CultureInfo.CurrentCulture);
                    break;
                case ViewTypeEnum.Int:
                    formatted = numericVal.ToString("N0", CultureInfo.CurrentCulture);
                    break;
            }

            var result = new NumericValue
            {
                Value = numericVal,
                Formatted = formatted
            };
            return Task.FromResult<object>(result);
        }
    }

    public class BooleanValueRenderer : IValueRenderer
    {
        public Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken ct)
        {
            bool val = propertyData.BoolValue ?? false;
            return Task.FromResult<object>(new BooleanValue { Value = val });
        }
    }

    public class DateValueRenderer : IValueRenderer
    {
        public async Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken ct = default)
        {
            if (propertyData?.Property == null)
                return null;

            var dateValue = new DateValue();

            var viewType = propertyData.Property.ViewType;
            if (!IsDateTimeViewType(viewType))
                return null;

            DateTime? rawDateTime = (DateTime?)propertyData.GetValue();


            if (rawDateTime == null || rawDateTime.Value == DateTime.MinValue)
            {
                dateValue.RawValue = null;
                dateValue.DisplayValue = "-";
                return dateValue;
            }

            switch (viewType)
            {
                case ViewTypeEnum.Time:
                    var timeSpan = rawDateTime.Value.TimeOfDay;
                    var timeResult = timeSpan.FormatTime();
                    dateValue.RawValue = rawDateTime;
                    dateValue.DisplayValue = timeResult.GetType()
                        .GetProperty("DisplayValue")?.GetValue(timeResult, null)?.ToString() ?? "-";
                    break;

                case ViewTypeEnum.Date:
                    var parseDateOnly = rawDateTime.Value.FormatDate();
                    dateValue.RawValue = parseDateOnly.RawValue;
                    dateValue.DisplayValue = parseDateOnly.DisplayValue;
                    break;

                case ViewTypeEnum.DateTime:
                    var parseDateTime = rawDateTime.Value.FormatDateTime();
                    dateValue.RawValue = parseDateTime.RawValue;
                    dateValue.DisplayValue = parseDateTime.DisplayValue;
                    break;

                case ViewTypeEnum.Month:
                    var parseMonth = rawDateTime.Value.FormatDateTime();
                    dateValue.RawValue = parseMonth.RawValue;
                    dateValue.DisplayValue = rawDateTime.Value
                        .AdjustToRegionDateTime().ToString("yyyy-MM", CultureInfo.CurrentCulture) ?? "-";
                    break;

                case ViewTypeEnum.Week:
                    var localWeekDate = rawDateTime.Value.AdjustToRegionDateTime();
                    if (localWeekDate != default)
                    {
                        var cal = CultureInfo.CurrentCulture.Calendar;
                        int w = cal.GetWeekOfYear(
                            localWeekDate,
                            CalendarWeekRule.FirstFourDayWeek,
                            DayOfWeek.Monday
                        );
                        dateValue.RawValue = localWeekDate;
                        dateValue.DisplayValue = $"{localWeekDate:yyyy}-W{w:00}";
                    }
                    else
                    {
                        dateValue.RawValue = null;
                        dateValue.DisplayValue = "-";
                    }
                    break;

                case ViewTypeEnum.Quarter:
                    var localQuarterDate = rawDateTime.Value.AdjustToRegionDateTime();
                    if (localQuarterDate != default)
                    {
                        int quarter = (localQuarterDate.Month - 1) / 3 + 1;
                        dateValue.RawValue = localQuarterDate;
                        dateValue.DisplayValue = $"{localQuarterDate.Year}-Q{quarter}";
                    }
                    else
                    {
                        dateValue.RawValue = null;
                        dateValue.DisplayValue = "-";
                    }
                    break;
            }

            return await Task.FromResult(dateValue);
        }

        private bool IsDateTimeViewType(ViewTypeEnum vt)
        {
            return vt == ViewTypeEnum.Date
                || vt == ViewTypeEnum.Time
                || vt == ViewTypeEnum.DateTime
                || vt == ViewTypeEnum.Month
                || vt == ViewTypeEnum.Week
                || vt == ViewTypeEnum.Quarter;
        }
    }

    public class UserValueRenderer : IValueRenderer
    {
        //private readonly IUserLookupService _userLookupService;
        //public UserValueRenderer(IUserLookupService userLookupService)
        //{
        //    _userLookupService = userLookupService;
        //}

        public async Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken ct)
        {
            var viewType = propertyData.Property.ViewType;
            //// Single user
            //if (viewType == ViewTypeEnum.User)
            //{
            //    if (!propertyData.GuidValue.HasValue)
            //        return null;

            //    var userId = propertyData.GuidValue.Value;
            //    var userRecord = await _userLookupService.GetUserByIdAsync(userId, ct);
            //    if (userRecord == null)
            //    {
            //        return new UserValue
            //        {
            //            Id = userId,
            //            DisplayName = "Unknown User"
            //        };
            //    }
            //    return new UserValue
            //    {
            //        Id = userRecord.Id,
            //        DisplayName = userRecord.DisplayName
            //    };
            //}
            //// MultiUser
            //if (viewType == ViewTypeEnum.MultiUser)
            //{
            //    if (string.IsNullOrEmpty(propertyData.StringValue))
            //    {
            //        return new MultiUserValue();
            //    }
            //    var userIds = JsonSerializer.Deserialize<List<Guid>>(propertyData.StringValue);
            //    var multiUser = new MultiUserValue();
            //    foreach (var uid in userIds)
            //    {
            //        var ur = await _userLookupService.GetUserByIdAsync(uid, ct);
            //        multiUser.Users.Add(new UserValue
            //        {
            //            Id = uid,
            //            DisplayName = ur?.DisplayName ?? "Unknown User"
            //        });
            //    }
            //    return multiUser;
            //}
            // fallback
            return null;
        }
    }

    public class LookupValueRenderer : IValueRenderer
    {
        //private readonly ILookupService _lookupService;
        //public LookupValueRenderer(ILookupService lookupService)
        //{
        //    _lookupService = lookupService;
        //}

        public async Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken ct)
        {
            //var vt = propertyData.Property.ViewType;
            //if (vt == ViewTypeEnum.Lookup)
            //{
            //    if (!propertyData.GuidValue.HasValue)
            //        return null;

            //    var item = await _lookupService.GetLookupItemAsync(propertyData.GuidValue.Value, ct);
            //    return new LookupValue
            //    {
            //        Id = item?.Id ?? propertyData.GuidValue.Value,
            //        DisplayName = item?.DisplayName ?? "Unknown Lookup"
            //    };
            //}
            //else if (vt == ViewTypeEnum.MultiLookup)
            //{
            //    if (string.IsNullOrEmpty(propertyData.StringValue))
            //        return new MultiLookupValue();

            //    var ids = JsonSerializer.Deserialize<List<Guid>>(propertyData.StringValue);
            //    var multi = new MultiLookupValue();
            //    foreach (var id in ids)
            //    {
            //        var item = await _lookupService.GetLookupItemAsync(id, ct);
            //        multi.Items.Add(new LookupValue
            //        {
            //            Id = item?.Id ?? id,
            //            DisplayName = item?.DisplayName ?? "Unknown Lookup"
            //        });
            //    }
            //    return multi;
            //}
            return null;
        }
    }

    public class AttachmentValueRenderer : IValueRenderer
    {
        //private readonly IAttachmentService _attachmentService;
        //public AttachmentValueRenderer(IAttachmentService attachmentService)
        //{
        //    _attachmentService = attachmentService;
        //}

        public async Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken ct)
        {
            //var vt = propertyData.Property.ViewType;
            //if (vt == ViewTypeEnum.Attachment)
            //{
            //    if (!propertyData.GuidValue.HasValue)
            //        return null;

            //    var attachId = propertyData.GuidValue.Value;
            //    var info = await _attachmentService.GetAttachmentAsync(attachId, ct);
            //    return new AttachmentValue
            //    {
            //        AttachmentId = attachId,
            //        FileName = info?.FileName ?? "Unknown",
            //        FileSize = info?.FileSize ?? 0,
            //        DownloadUrl = info?.DownloadUrl ?? ""
            //    };
            //}
            //else if (vt == ViewTypeEnum.MultiAttachment)
            //{
            //    if (string.IsNullOrEmpty(propertyData.StringValue))
            //        return new MultiAttachmentValue();

            //    var attachIds = propertyData.StringValue.FromJson<List<Guid>>();

            //    var multiVal = new MultiAttachmentValue();
            //    foreach (var id in attachIds)
            //    {
            //        var info = await _attachmentService.GetAttachmentAsync(id, ct);
            //        multiVal.Attachments.Add(new AttachmentValue
            //        {
            //            AttachmentId = id,
            //            FileName = info?.FileName ?? "Unknown",
            //            FileSize = info?.FileSize ?? 0,
            //            DownloadUrl = info?.DownloadUrl ?? ""
            //        });
            //    }
            //    return multiVal;
            //}
            return null;
        }
    }

    public class DynamicValueRenderer : IValueRenderer
    {
        public Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken ct)
        {
            // For "Api", "Dynamic", "MappedPropertyList", etc.
            var vt = propertyData.Property.ViewType;
            return vt switch
            {
                ViewTypeEnum.Api => RenderApiValue(propertyData),
                ViewTypeEnum.Dynamic => RenderDynamic(propertyData),
                ViewTypeEnum.MappedPropertyList => RenderMappedPropertyList(propertyData),
                ViewTypeEnum.ExternalDisplayValue => RenderExternalDisplayValue(propertyData),
                _ => Task.FromResult<object>(null),
            };
        }

        private Task<object> RenderApiValue(PropertyData propertyData)
        {
            if (string.IsNullOrEmpty(propertyData.StringValue))
                return Task.FromResult<object>(new DynamicValue { Data = null });

            return Task.FromResult<object>(new DynamicValue { Data = null });
        }

        private Task<object> RenderDynamic(PropertyData propertyData)
        {
            if (string.IsNullOrEmpty(propertyData.StringValue))
                return Task.FromResult<object>(new DynamicValue { Data = null });

            return Task.FromResult<object>(new DynamicValue { Data = null });
        }

        private Task<object> RenderMappedPropertyList(PropertyData propertyData)
        {
            if (string.IsNullOrEmpty(propertyData.StringValue))
                return Task.FromResult<object>(new MappedPropertyListValue());

            return Task.FromResult<object>(null);
        }

        private Task<object> RenderExternalDisplayValue(PropertyData propertyData)
        {
            return Task.FromResult<object>(null);
        }
    }

    public class ConnectionForeignKeyRenderer : IValueRenderer
    {
        public Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken ct)
        {
            switch (propertyData.Property.ViewType)
            {
                case ViewTypeEnum.Connection:
                    return RenderConnection(propertyData);
                case ViewTypeEnum.ForeignKey:
                    return RenderForeignKey(propertyData);
                case ViewTypeEnum.ModuleReference:
                    return RenderModuleReference(propertyData);
                default:
                    return Task.FromResult<object>(null);
            }
        }

        private Task<object> RenderConnection(PropertyData propertyData)
        {
            if (!propertyData.GuidValue.HasValue)
                return Task.FromResult<object>(null);
            return Task.FromResult<object>(null);
        }

        private Task<object> RenderForeignKey(PropertyData propertyData)
        {
            if (!propertyData.GuidValue.HasValue)
                return Task.FromResult<object>(null);
            return Task.FromResult<object>(null);
        }

        private Task<object> RenderModuleReference(PropertyData propertyData)
        {
            return Task.FromResult<object>(null);
        }
    }
}
