using BenchmarkDotNet.Running;
using EVotingSystem.Comparison.Dotnet;

var summary = BenchmarkRunner.Run<EncryptionServiceBenchmarks>();