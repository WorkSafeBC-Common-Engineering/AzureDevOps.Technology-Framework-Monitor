// See https://aka.ms/new-console-template for more information
using Parser.Interfaces;

using ProjectData;

Console.WriteLine("Hello, World!");

var filename = args[0];
string[] fileData = File.ReadAllLines(filename);

IFileParser parser = new YamlFileParser.PipelineParser();
var fileItem = new FileItem();

parser.Parse(fileItem, fileData);
Console.WriteLine($"Template: {fileItem.PipelineProperties["template"]}\nPortfolio: {fileItem.PipelineProperties["portfolio"]}\nProduct: {fileItem.PipelineProperties["product"]}");