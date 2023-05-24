# Working guide for Semantic Kernel

This document is created in the course of learning Senamtic Kernel in a dot net based environment.
This does not work with demos, instead it is developed as an independent app from scratch.
This document flows like a conversation with code snippets in between. Note that before running any
command you also run all the commands that appeared before it. Commands will be interspersed with notes, comments and discussions.

`dotnet new console`

## Adding Semantic Kernel (SK) to a dotnet project
`dotnet add package Microsoft.SemanticKernel --prerelease`

## A sample program

Program.cs
```C#
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration; // for ContextVariables

namespace SemanticKernelLearning
{
    public static class SemanticKernelMain
    {
        public static async Task Main()
        {
            var kernel = Kernel.Builder.Build();
            kernel.Config.AddOpenAITextCompletionService("text-davinci-003", "sk-SECRET-GOES-HERE");
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
            Console.WriteLine($"Business summary\n{result}");
            context = new ContextVariables();
            context.Set("FORMAT","XML");
            context.Set("STORY", story);
            result = await kernel.RunAsync(context, plugin["StorySummarizer"]);
            Console.WriteLine($"Story summary\n{result}");
        }
    }
}
```
CharacterIntention/skprompt.txt
```
Summarize the intention of every character in the story written between triple dashes (---) and output the result as json with keys character_name and intention_brief.
---
{{$INPUT}}
---
```
BusinessSummary/skprompt.txt
```
Generate a {{FORMAT}} for {{BUSINESS_NAME}} which is located in {{BUSINESS_COUNTRY}} and serves {{BUSINESS_GEOGRAPHY}} customers using keys name, orgLocation and customerLocation.
```

Folder structure -

    root
        LearningPlugin
            CharacterIntention
                skprompt.txt
                config.json
            BusinessSummary
                skprompt.txt
                config.json
        Program.cs

Note - Simply calling GPT with a prompt is called **Semantic Function** in the realm of Semantic Kernel (SK). Similarly a function with conventional code is called a **Native Function**. In the above example *CharacterIntention* is a semantic function. The input to a semantic function is specified using the `{{INPUT}}` placeholder.

Note - A semantic function is written as a skprompt.txt and a config.json file in a directory. The directory name is the function name.

## Model size and usages

| Model Name | No. of Parameters | Corpus Size | Good for                                                                                                                                                    |
| ---------- | ----------------- | ----------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Ada        | 350M              | 40 GB       | Classification, Sentiment Analysis, Summarization, Simple Conversation                                                                                      |
| Babbage    | 3B                | 300 GB      | Reasoning, Logic, Arithmetic, Word Analogy                                                                                                                  |
| Curie      | 13B               | 800 GB      | Text-to-speech, Speech-to-text, Translation, Paraphrasing, Question asnwering                                                                               |
| Davinci    | 175B              | 45 TB       | Image captioning, Style transfer, Visual reasoning, Generate coherent and creative texts on any topic with considerable fluency, consistency and diversity. |

## GPT configurations (config.json files)

GPT models provide a number of levers to control the output of the completion. Semantic Kernel also adds one viz **description** which is used by the planner (a component of SK SDK). These parameters should be used as follows.

| Parameter Name | Description                                                                            | Remark                                                                                                                                                  |
| -------------- | -------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------- |
| max_tokens     | the maximum number of tokens produce                                                   | the model's context length **includes both** the length of the prompt and the number of tokens generated as output                                      |
| temperature    | called Sampling Temperature, this parameter controls how much risk a model will take   | Try 0.9 for more creative applications, and 0 (argmax sampling) for ones with a well-defined answer., adjust either this or *top_p* parameter, not both |
| top_p          | called nucleus sampling, this parameter ensures that new tokens are used in the prompt | e.g. a value of 0.1 indicates that only token comprising of top 10% probability mass are considered                                                     |
| presence_penalty | controls how much a model is penalized for not using new tokens | Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far, increasing the model's likelihood to talk about new topics.|
| frequency_penalty | controls how much a model is penalized for repeating a token in output | Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so far, decreasing the model's likelihood to repeat the same line verbatim. |
| description | a concise statement describing what the funciton does | this parameter is used by the Planner component in SK SDK to plan the steps in execution of an ASK |
| input | a json object describing the inputs to the semantic function | example `"input": {"parameters":[{"name": "input","description":"The product to generate a slogan for","defaultValue":""}]}`
|

Note: Frequency_penalty and presence_penalty are two parameters that can be used when generating text with language models, such as GPT-3.

1. Frequency_penalty: This parameter is used to discourage the model from repeating the same words or phrases too frequently within the generated text. It is a value that is added to the log-probability of a token each time it occurs in the generated text. **A higher frequency_penalty value will result in the model being more conservative in its use of repeated tokens.**

2. Presence_penalty: This parameter is used to encourage the model to include a diverse range of tokens in the generated text. It is a value that is subtracted from the log-probability of a token each time it is generated. **A higher presence_penalty value will result in the model being more likely to generate tokens that have not yet been included in the generated text.**

Both of these parameters can be adjusted to influence the overall quality and diversity of the generated text. The optimal values for these parameters may vary depending on the specific use case and desired output.

## Sampling

GPT models convert text into embeddings, which are long array of real numbers. Tho do this the text is broken into tokens. There are many ways to get token from text, each with its own pros and cons. Token can be words, characters or sub-words. Tokens define how much memory and cost the training will take as well as the generality and diversity of generated text.

OpenAI and Azure OpenAI uses a subword tokenization method called "Byte-Pair Encoding (BPE)" for its GPT-based models. BPE is a method that merges the most frequently occurring pairs of characters or bytes into a single token, until a certain number of tokens or a vocabulary size is reached.

Ada has the smallest vocabulary size, with 50,000 tokens, and Davinci has the largest vocabulary size, with 60,000 tokens. Babbage and Curie have the same vocabulary size, with 57,000 tokens. The larger the vocabulary size, the more diverse and expressive the texts that the model can generate. However, the larger the vocabulary size, the more memory and computational resources that the model requires.