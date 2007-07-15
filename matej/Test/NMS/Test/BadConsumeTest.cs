/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using NUnit.Framework;

namespace NMS.Test
{
    [TestFixture]
    public abstract class BadConsumeTest : NMSTestSupport
    {
        [SetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TearDown]
        public override void TearDown()
        {
            base.TearDown();
        }

        [Test]
        public void TestBadConsumeOperationToTestExceptions()
        {
            try
            {
                IMessageConsumer consumer = Session.CreateConsumer(null);
                Console.WriteLine("Created consumer: " + consumer);
                Assert.Fail("Should  have thrown an exception!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Caught expected exception: " + e);
                Console.WriteLine("Stack: " + e.StackTrace);
            }
        }
    }
}