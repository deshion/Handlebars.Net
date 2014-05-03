﻿using System;
using System.IO;
using Handlebars.Compiler;
using System.Text;

namespace Handlebars
{
    public delegate void HandlebarsHelper(TextWriter output, params object[] arguments);
    public delegate void HandlebarsBlockHelper(TextWriter output, Action<TextWriter, object> template, params object[] arguments);

    public sealed class Handlebars
    {
        private static readonly HandlebarsConfiguration _configuration = new HandlebarsConfiguration();
        private static readonly HandlebarsCompiler _compiler = new HandlebarsCompiler(_configuration);

        public static Action<TextWriter, object> Compile(TextReader template)
        {
            return _compiler.Compile(template);
        }

        public static Func<object, string> Compile(string template)
        {
            using(var reader = new StringReader(template))
            {
                var compiledTemplate = Compile(reader);
                return context => {
                    var builder = new StringBuilder();
                    using(var writer = new StringWriter(builder))
                    {
                        compiledTemplate(writer, context);
                    }
                    return builder.ToString();
                };
            }
        }

        public static void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            if(_configuration.Helpers.ContainsKey(helperName) == false)
            {
                lock(typeof(Handlebars))
                {
                    if(_configuration.Helpers.ContainsKey(helperName) == false)
                    {
                        _configuration.Helpers.Add(helperName, helperFunction);
                    }
                }
            }
        }

        public static void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            if(_configuration.BlockHelpers.ContainsKey(helperName) == false)
            {
                lock(typeof(Handlebars))
                {
                    if(_configuration.BlockHelpers.ContainsKey(helperName) == false)
                    {
                        _configuration.BlockHelpers.Add(helperName, helperFunction);
                    }
                }
            }
        }
    }
}

