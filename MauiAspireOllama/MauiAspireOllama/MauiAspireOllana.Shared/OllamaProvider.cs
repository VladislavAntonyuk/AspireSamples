namespace MauiAspireOllana.Shared;

using System.Text.Json;
using Microsoft.Extensions.Logging;
using Ollama;

public class OllamaProvider(IOllamaApiClient api, ILogger<OllamaProvider> logger)
{
	public async Task<T?> GetResponse<T>(string model, string request)
	{
		var result = await api.Chat.GenerateChatCompletionAsync(new GenerateChatCompletionRequest()
		{
			Model = model,
			Messages = [
				new Message(MessageRole.System, "You are a meteostation with a great knowledge of weather."),
					new Message(MessageRole.User, request)
			],
			Format = ResponseFormat.Json
		});

		logger.LogInformation("Received a response from AI: {Response}, Duration: {Duration}",
							  result.Message.Content, result.TotalDuration);
		var response = result.Message.Content;
		return JsonSerializer.Deserialize<T>(response, new JsonSerializerOptions(JsonSerializerDefaults.Web));
	}

	public async Task<string?> GetImageResponse(string model, string request)
	{
		try
		{
			var result = await api.Chat.GenerateChatCompletionAsync(new GenerateChatCompletionRequest()
			{
				Model = model,
				Messages = [new Message(MessageRole.User, request)]
			});

			return result.Message.Images?.FirstOrDefault();
		}
		catch (Exception e)
		{
			logger.LogError(e, "Failed generating image for {Request}", request);
		}

		return null;
	}

	public async Task PullModelAsync(string model)
	{
		var runningModels = await api.Models.ListModelsAsync();
		if (runningModels.Models?.Select(x => x.Model1).Contains(model) == true)
		{
			return;
		}

		await api.Models.PullModelAsync(model);
	}
}