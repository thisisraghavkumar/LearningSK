// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration; // for ContextVariables

namespace SemanticKernelLearning
{
        public static class SemanticKernelMain {
        public static async Task Main()
        {
            var kernel = Kernel.Builder.Build();
            kernel.Config.AddOpenAITextCompletionService("text-davinci-003", "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");
            Console.WriteLine("Initiated kernel...");
            var plugin = kernel.ImportSemanticSkillFromDirectory(Directory.GetCurrentDirectory(),"LearningPlugin");
            Console.WriteLine("Loadeded plugin...");
            var story = "Alladin was waiting for an opportunity to show to the King his prowess with sword and diplomacy. By convincing the King that he's a worthy suitor for the former's daughter he'd be able to make a compelling case while asking for Jasmine's hand in marraige. Jasmine also loved Alladin but was scared to confess her feelings out loud, even to Alladin. So she would surreptiously plead in his favour infront of the King. The old King was worried about the future of his state and his daughter. He was actively seeking matches for his dauther. Meanwhile the evil Vizier Vladimir was plotting to usurp the throne.";
            var result = await kernel.RunAsync(story, plugin["CharacterIntention"]);
            Console.WriteLine(result);
            var context = new ContextVariables();
            context.Set("FORMAT","XML");
            context.Set("BUSINESS_NAME","Contoso");
            context.Set("BUSINESS_COUNTRY", "India");
            context.Set("BUSINESS_GEOGRAPHY", "Globally");
            result = await kernel.RunAsync(context, plugin["BusinessSummary"]);
        }
    }
}