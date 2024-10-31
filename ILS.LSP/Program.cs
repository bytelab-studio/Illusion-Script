using System.Threading.Tasks;

using ILS.Parsing;
using OmniSharp.Extensions.LanguageServer.Protocol.Document;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Server;

namespace ILS.LSP;

public static class Program
{
    public static async Task Main(string[] args)
    {
        // LanguageServer server = await LanguageServer.From(
        //     options => options
        //         .WithInput(Console.OpenStandardInput())
        //         .WithOutput(Console.OpenStandardOutput())
        //         .OnInitialize((server, request, token) =>
        //         {
        //             Console.WriteLine("Language Server initialized");
        //             return Task.CompletedTask;
        //         })
        //         .OnShutdown(shutdownParams => Task.CompletedTask)
        //         .OnHover((hoverParams, token) =>
        //         {
        //             return Task.FromResult(new Hover
        //             {
        //                 Contents = new MarkedStringsOrMarkupContent(
        //                     new MarkedString("ILS", "ILS ILS ILS ILS is the ILSest"))
        //             });
        //         }, (capability, capabilities) => new HoverRegistrationOptions())
        //         .OnDidOpenTextDocument(documentParams =>
        //         {
        //             Parser parser = new Parser()
        //         })
        // );
        //
        // await server.WaitForExit;
    }
}