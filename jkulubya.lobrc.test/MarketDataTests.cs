using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace jkulubya.lobrc.test
{
    [Parallelizable(ParallelScope.Children)]
    public class MarketDataTests
    {
        [Test]
        public void FullExecution()
        {
            var mreader = new TestReader(new List<Message>
            {
                Message.New(new LimitOrder("1", "symbol", false, 100, 100), DateTimeOffset.Now),
                Message.Execution("1", 100, DateTimeOffset.Now)
            });
            
            var mwriters = new List<IMessageWriter>() {new MessageWriter()};

            var obWriter = new OrderBookWriter();
            
            var controller = new Controller(mreader, mwriters, obWriter, new NullLogger<string>());
            controller.Start();
            Thread.Sleep(1000);
            controller.Stop();

            {
                var ob = obWriter.OrderBooks.Last();
                Assert.IsEmpty(ob.Bids);
            }
        }

        [Test]
        public void PartialExecution()
        {
            var mreader = new TestReader(new List<Message>
            {
                Message.New(new LimitOrder("1", "symbol", true, 100, 100), DateTimeOffset.Now),
                Message.Execution("1", 50, DateTimeOffset.Now)
            });
            
            var mwriters = new List<IMessageWriter>() {new MessageWriter()};

            var obWriter = new OrderBookWriter();
            
            var controller = new Controller(mreader, mwriters, obWriter, new NullLogger<string>());
            controller.Start();
            Thread.Sleep(1000);
            controller.Stop();

            {
                var ob = obWriter.OrderBooks.Last();
                Assert.AreEqual(1, ob.Bids.Count());
                Assert.AreEqual(50, ob.Bids.Single().Quantity);
            }
        }

        [Test]
        public void Cancellation()
        {
            var mreader = new TestReader(new List<Message>
            {
                Message.New(new LimitOrder("1", "symbol", false, 100, 100), DateTimeOffset.Now),
                Message.Delete("1", DateTimeOffset.Now)
            });
            
            var mwriters = new List<IMessageWriter>() {new MessageWriter()};

            var obWriter = new OrderBookWriter();
            
            var controller = new Controller(mreader, mwriters, obWriter, new NullLogger<string>());
            controller.Start();
            Thread.Sleep(1000);
            controller.Stop();

            {
                var ob = obWriter.OrderBooks.Last();
                Assert.IsEmpty(ob.Asks);
            }
        }

        [Test]
        public void Bbo()
        {
            var mreader = new TestReader(new List<Message>
            {
                Message.New(new LimitOrder("1", "symbol", false, 100, 100), DateTimeOffset.Now),
                Message.Execution("1", 50, DateTimeOffset.Now),
                Message.New(new LimitOrder("2", "symbol", true, 101, 20), DateTimeOffset.Now),
                Message.New(new LimitOrder("3", "symbol", true, 101, 50), DateTimeOffset.Now),
                Message.New(new LimitOrder("4", "symbol", false, 99, 45), DateTimeOffset.Now),
                Message.New(new LimitOrder("5", "symbol", false, 98, 150), DateTimeOffset.Now),
                Message.Execution("1", 50, DateTimeOffset.Now),
                Message.Execution("2", 10, DateTimeOffset.Now),
                Message.New(new LimitOrder("6", "symbol", true, 99, 500), DateTimeOffset.Now),
            });
            
            var mwriters = new List<IMessageWriter> {new MessageWriter()};

            var obWriter = new OrderBookWriter();
            
            var controller = new Controller(mreader, mwriters, obWriter, new NullLogger<string>());
            controller.Start();
            Thread.Sleep(1000);
            controller.Stop();

            {
                Assert.AreEqual(9, obWriter.OrderBooks.Count);
                var ob = obWriter.OrderBooks.Last();
                
                Assert.AreEqual(98, ob.GetAskPrice(0));
                Assert.AreEqual(150, ob.GetPriceVolume(0).Item2);
                
                Assert.AreEqual(101, ob.GetBidPrice(0));
                Assert.AreEqual(60, ob.GetPriceVolume(0).Item1);
            }
        }
    }
}