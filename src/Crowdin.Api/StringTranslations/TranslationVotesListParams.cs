﻿
using System.Collections.Generic;
using Crowdin.Api.Core;
using JetBrains.Annotations;

#nullable enable

namespace Crowdin.Api.StringTranslations
{
    [PublicAPI]
    public class TranslationVotesListParams : IQueryParamsProvider
    {
        public int? StringId { get; set; }

        public string? LanguageId { get; set; }
        
        public int? TranslationId { get; set; }

        public int Limit { get; set; } = 25;
        
        public int Offset { get; set; }

        public TranslationVotesListParams()
        {
            
        }

        public TranslationVotesListParams(
            int? stringId,
            string? languageId,
            int? translationId,
            int limit, int offset)
        {
            StringId = stringId;
            LanguageId = languageId;
            TranslationId = translationId;
            Limit = limit;
            Offset = offset;
        }

        public IDictionary<string, string> ToQueryParams()
        {
            IDictionary<string, string> queryParams =
                Utils.CreateQueryParamsFromPaging(Limit, Offset);
            
            queryParams.AddParamIfPresent("stringId", StringId);
            queryParams.AddParamIfPresent("languageId", LanguageId);
            queryParams.AddParamIfPresent("translationId", TranslationId);

            return queryParams;
        }
    }
}