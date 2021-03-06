﻿#region Licence
/* The MIT License (MIT)
Copyright © 2015 Ian Cooper <ian_hammond_cooper@yahoo.co.uk>

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the “Software”), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

using System.Linq;
using nUnitShouldAdapter;
using NUnit.Specifications;
using paramore.brighter.commandprocessor.tests.nunit.CommandProcessors.TestDoubles;
using TinyIoC;

namespace paramore.brighter.commandprocessor.tests.nunit.CommandProcessors
{
    [Subject(typeof(PipelineBuilder<>))]
    public class When_A_Handler_Is_Part_Of_An_Async_Pipeline : ContextSpecification
    {
        private static PipelineBuilder<MyCommand> s_pipelineBuilder;
        private static IHandleRequestsAsync<MyCommand> s_pipeline;

        private Establish _context = () =>
        {
            var registry = new SubscriberRegistry();
            registry.RegisterAsync<MyCommand, MyImplicitHandlerAsync>();

            var container = new TinyIoCContainer();
            var handlerFactory = new TinyIocHandlerFactoryAsync(container);
            container.Register<IHandleRequestsAsync<MyCommand>, MyImplicitHandlerAsync>();
            container.Register<IHandleRequestsAsync<MyCommand>, MyLoggingHandlerAsync<MyCommand>>();

            s_pipelineBuilder = new PipelineBuilder<MyCommand>(registry, handlerFactory);
        };

        private Because _of = () => s_pipeline = s_pipelineBuilder.BuildAsync(new RequestContext(), false).First();

        private It _should_include_my_command_handler_filter_in_the_chain = () => TracePipeline().ToString().Contains("MyImplicitHandlerAsync").ShouldBeTrue();
        private It _should_include_my_logging_handler_in_the_chain = () => TracePipeline().ToString().Contains("MyLoggingHandlerAsync").ShouldBeTrue();

        private static PipelineTracer TracePipeline()
        {
            var pipelineTracer = new PipelineTracer();
            s_pipeline.DescribePath(pipelineTracer);
            return pipelineTracer;
        }
    }
}
