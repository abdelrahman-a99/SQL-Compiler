using Microsoft.AspNetCore.Mvc;
using SQL_Compiler.Models;
using System.Collections.Generic;
using System.Linq;

namespace SQL_Compiler.Controllers
{
    public class LexerController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View();

        // Accept JSON body (AJAX) and return tokens with readable Type as string
        [HttpPost]
        public IActionResult Analyze([FromBody] string inputCode)
        {
            if (string.IsNullOrWhiteSpace(inputCode))
                return BadRequest("Please enter SQL-like code.");

            var lexer = new Lexer();
            var tokens = lexer.Analyze(inputCode);

            // Project to anonymous objects with Type as string
            var result = tokens.Select(t => new
            {
                type = t.Type.ToString(),   // string value of enum
                lexeme = t.Lexeme,
                line = t.Line,
                column = t.Column
            }).ToList();

            return Json(result);
        }
    }
}
