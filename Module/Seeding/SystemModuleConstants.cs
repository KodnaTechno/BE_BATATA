namespace Module.Seeding
{

    public static class SystemApplicationConstants
    {
        public static readonly Guid AssetManagementApplicationId = new("A1B2C3D4-E5F6-47AE-8EB7-D1E1E83A6F9C");
    }

    public static class SystemWorkspaceConstants
    {
        public static class AssetManagement
        {
            public static readonly Guid LocationWorkspaceId = new("E9A8748E-41D5-4C31-9C5C-52A10C4F7420");

            public static class Properties
            {
                public static readonly Guid CreatedAtId = new("2392D348-6AF0-4B67-9271-BA6A8AEEBED0");
                public static readonly Guid CreatedById = new("FB23F579-E069-4ECC-BBFD-58EBE8DD2350");
                public static readonly Guid UpdatedAtId = new("2224FAE6-3DFF-45C9-8FD2-6996FB14E9E0");
                public static readonly Guid UpdatedById = new("A73E4CE5-50B6-4A79-A979-81722B6D4352");
                public static readonly Guid DeletedAtId = new("71C58090-D4C0-4BEE-8B9B-417F938DE7F4");
                public static readonly Guid DeletedById = new("F2A4B262-5E35-4CE7-98CA-E4AF8C08CC60");


                public static readonly Guid LocationNameId = new("A2F61DE5-C906-4A0E-8A79-37A119FB6A59");
                public static readonly Guid AddressId = new("B753054D-75A9-4C48-9FE8-C5704459E579");
                public static readonly Guid CityId = new("24A02E7E-5872-4E29-9DB4-3AE3C3A7A6C1");
                public static readonly Guid CountryId = new("3CE24AD1-1E3F-4CC6-B0AC-A0CBD00F7F03");
                public static readonly Guid LatitudeId = new("DD8A1A35-1B7C-4DD7-8AB7-4C4D9FF4F870");
                public static readonly Guid LongitudeId = new("EE1B3C39-2D3A-4A89-AF98-22D629092BA5");
                public static readonly Guid IsActiveId = new("665F8454-3B41-4D4B-9C76-18BBA18ECF2A");
            }
        }
    }

    public static class SystemModuleConstants
    {
        public static class Basic
        {
            public static class TaskModule
            {
                public static readonly Guid Id = new("89A9748E-41D5-4C31-9C5C-52A10C4F7419");

                public static class Properties
                {
                    public static readonly Guid CreatedAtId = new("63E6128A-2903-4500-A2E8-15AF07867DF3");
                    public static readonly Guid CreatedById = new("E0A3BBFF-5314-41FE-9A9D-5B13B2151A67");
                    public static readonly Guid UpdatedAtId = new("1894B31A-C4C4-411E-B116-E3D3EA0D5124");
                    public static readonly Guid UpdatedById = new("81CC79F8-200B-49BC-AC71-B6D2E19B4CC4");
                    public static readonly Guid DeletedById = new("64B8369E-497F-462D-BC30-AC97C3E43B30");
                    public static readonly Guid DeletedAtId = new("EE82A724-8AA7-412D-ADD7-CFC25B4D15F6");
                    public static readonly Guid TitleId = new("9D6B2976-C5EA-4C7A-91E7-C684F3B57F33");
                    public static readonly Guid AssignedToId = new("F1F61DE5-C906-4A0E-8A79-37A119FB6A54");
                    public static readonly Guid DueDateId = new("B653054D-75A9-4C48-9FE8-C5704459E578");
                }
            }
        }
    }



}
