namespace AppCommon.EnumShared
{ 
    public enum ViewTypeEnum
    {
        Text, 
        LongText,
        RichText,
        Int, 
        Float,
        Currency,
        Percentage,
        User,
        MultiUser,
        Lookup,
        MultiLookup,
        CheckBox,
        Api,
        Date,
        Time,
        DateTime,
        Month,
        Week,
        Quarter,
        ExternalDisplayValue, 
        MappedPropertyList,
        Dynamic,
        Attachment,
        MultiAttachment,
        Connection,
        ForeignKey,
        ModuleReference // => {"ReferenceModuleId":"", "Multiple": true, false}
    }
}
