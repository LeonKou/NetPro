using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetPro.Web.Core.Providers
{
    public class CustomMetadataProvider : IMetadataDetailsProvider, IDisplayMetadataProvider
    {
        public void CreateDisplayMetadata(DisplayMetadataProviderContext context)
        {
            if (context.Key.MetadataKind == ModelMetadataKind.Property)
            {
                context.DisplayMetadata.ConvertEmptyStringToNull = false;
            }
        }
    }
}
