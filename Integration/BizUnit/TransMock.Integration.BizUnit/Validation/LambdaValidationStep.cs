﻿/***************************************
//   Copyright 2016 - Svetoslav Vasilev

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
*****************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BizUnit;
using BizUnit.TestSteps;
using BizUnit.Core.TestBuilder;

namespace TransMock.Integration.BizUnit.Validation
{
    /// <summary>
    /// Implements logic for lambda expression validation
    /// </summary>
    public class LambdaValidationStep : SubStepBase
    {
        /// <summary>
        /// The lambda expression that will be executing the validation logic
        /// </summary>
        public Func<Stream, bool> ValidationCallback { get; set; }

        /// <summary>
        /// Validates if the step instance is correctly configured
        /// </summary>
        /// <param name="context">The instance of the BizUnit test context</param>
        public override void Validate(Context context)
        {
            if (ValidationCallback == null)
            {
                throw new ArgumentNullException("Validatipon expression cannot be null!");
            }            
        }

        /// <summary>
        /// Invokes the validation callback to perform the validation logic
        /// </summary>
        /// <param name="data">The stream containing the message to be validated</param>
        /// <param name="context">The instance of the BizUnit test context</param>
        /// <returns>The stream containing the message that was validated</returns>
        public override Stream Execute(Stream data, Context context)
        {
            bool result = ValidationCallback(data);

            if (!result)
            {
                throw new ValidationStepExecutionException("Validation in a lambda expression failed!", context.TestName);
            }
            // Rewinding the stream just in case
            data.Seek(0, SeekOrigin.Begin);

            return data;
        }
    }
}
