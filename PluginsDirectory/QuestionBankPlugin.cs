using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Orchestration;
using System.Net;
using System.Net.Http;
using Newtonsoft

namespace SemanticKernelLearning {
    public class QuestionBankPlugin {
        [SKFunction("Adds questions to the question back for a test")]
        [SKFunctionName("PostQuestions")]
        public async Task<bool> PostQuestionsAsync(string input) {
            var httpClient = new HttpClient();
            
        }
    }
}