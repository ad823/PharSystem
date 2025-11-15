using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections.Concurrent;

public class ModelManager
{
    private static readonly ConcurrentDictionary<string, InferenceSession> _models = new();
    private static readonly object _lock = new();

    /// <summary>
    /// 初始化模型
    /// </summary>
    public static void InitializeModel(string modelName, string modelPath, ILogger logger)
    {
        if (!_models.ContainsKey(modelName))
        {
            lock (_lock)
            {
                if (!_models.ContainsKey(modelName))
                {
                    try
                    {
                        var options = new SessionOptions();
                        options.GraphOptimizationLevel = GraphOptimizationLevel.ORT_ENABLE_ALL;
                        options.IntraOpNumThreads = 4;
                        options.InterOpNumThreads = 2;
                        options.ExecutionMode = ExecutionMode.ORT_PARALLEL;
                        //options.AppendExecutionProvider_CUDA(0); // 啟用 CUDA（如需要）
                        //options.EnableProfiling = true;
                        //options.EnableMemoryPattern = true;
                        //options.ProfileOutputPathPrefix = "profiling.json";

                        logger.LogInformation($"正在載入模型: {modelName}...");
                        var session = new InferenceSession(modelPath, options);
                        _models[modelName] = session;
                        logger.LogInformation($"模型 {modelName} 載入完成");
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"模型 {modelName} 初始化失敗");
                        throw;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 獲取指定名稱的模型 Session
    /// </summary>
    public static InferenceSession GetModel(string modelName)
    {
        if (_models.TryGetValue(modelName, out var session))
        {
            return session;
        }

        throw new InvalidOperationException($"模型 {modelName} 尚未初始化！");


    }
  
}
