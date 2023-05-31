using System;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;

namespace SemanticKernelLearning {
    public class NativePlugin {
        [SKFunction("Get a secret value from environment")]
        public string GetSecretFromEnvironment(string key)
        {
            return Environment.GetEnvironmentVariable(key) ?? "no val";
        }

        [SKFunction("Normalizes a string by converting it to lowercase and removing spaces")]
        public string NormalizeString(string input)
        {
            return input.Trim().ToLower();
        }

        [SKFunction("Look for a substring in a base string")]
        [SKFunctionContextParameter(Name="BaseString", Description = "The string in which the substring will be looked for")]
        [SKFunctionContextParameter(Name="SearchString", Description = "The string which will be looked for in the base string")]
        public string LookFor(SKContext ctx)
        {
            var baseString = NormalizeString(ctx["BaseString"]);
            var searchString = NormalizeString(ctx["SearchString"]);
            return baseString.IndexOf(searchString) != -1 ? "found": "not found";
        }

        [SKFunction("A native wrapper for a semantic function Business Summary")]
        [SKFunctionContextParameter(Name ="FORMAT", Description = "The format in which to output the resultant information")]
        [SKFunctionContextParameter(Name ="BUSINESS_NAME", Description = "The name of the business that will be associated with the key name")]
        [SKFunctionContextParameter(Name ="BUSINESS_COUNTRY", Description = "The name of the country where their office is located. This will be associated with the key orgLocation.")]
        [SKFunctionContextParameter(Name ="BUSSINESS_GEOGRAPHY", Description = "The name of the region where the business's customers are located. This will be associated with the customerLocation key.")]
        public async Task<string> BusinessSummaryWrapperAsync(SKContext ctx)
        {
            // var bsummary = ctx.Skills.GetFunction("LearningPlugin", "BusinessSummary"); // both these lines have the same effect
            var bsummary = ctx.Func("LearningPlugin", "BusinessSummary"); // both these lines have the same effect
            var res = await bsummary.InvokeAsync(ctx);
            return res.Result.ReplaceLineEndings(";");
        }

    }
}