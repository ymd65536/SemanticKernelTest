using Azure.Identity;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

// Kernel builder を作る
var builder = Kernel.CreateBuilder();

// todo: endpointプロパティははあとで環境変数から参照できるように変更しておく
// kernel が使う AI モデル情報,認証情報などを登録
builder.AddAzureOpenAIChatCompletion(
        deploymentName: "gpt-35-turbo",
        modelId: "gpt-35-turbo",
        endpoint: "https://{test_endpoint}.openai.azure.com",
        credentials: new DefaultAzureCredential()
);

var kernel = builder.Build();


var skPrompt = @"{{$input}}
上の文章を、なるべく短く要約してください。";

var executionSettings = new OpenAIPromptExecutionSettings 
{
    MaxTokens = 2000,
    Temperature = 0.2,
    TopP = 0.5
};

var input = """
吾輩は猫である。名前はまだ無い。
どこで生れたかとんと見当がつかぬ。何でも薄暗いじめじめした所でニャーニャー泣いていた事だけは記憶している。
吾輩はここで始めて人間というものを見た。
しかもあとで聞くとそれは書生という人間中で一番獰悪な種族であったそうだ。
この書生というのは時々我々を捕えて煮て食うという話である。
しかしその当時は何という考もなかったから別段恐しいとも思わなかった。
ただ彼の掌に載せられてスーと持ち上げられた時何だかフワフワした感じがあったばかりである。
掌の上で少し落ちついて書生の顔を見たのがいわゆる人間というものの見始であろう。
""";

var summaryFunction = kernel.CreateFunctionFromPrompt(
    promptTemplate: skPrompt, 
    executionSettings: executionSettings
);

var summaryResult = await kernel.InvokeAsync(
    summaryFunction, 
    new KernelArguments() { ["input"] = input }
);

Console.WriteLine(summaryResult);
