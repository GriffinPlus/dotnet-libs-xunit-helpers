///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// This file is part of the Griffin+ common library suite (https://github.com/GriffinPlus/dotnet-libs-xunit-helpers)
//
// Copyright 2019 Sascha Falk <sascha@falk-online.eu>
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance
// with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed
// on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for
// the specific language governing permissions and limitations under the License.
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace GriffinPlus.Lib.Tests
{
	/// <summary>
	/// Helper functions for unit tests covering asynchronous code.
	/// </summary>
	public class AsyncAssert
	{
		/// <summary>
		/// Ensures that the specified task does not complete within the specified time.
		/// Raises a <see cref="Exception"/>, if the task completes before the specified timeout.
		/// </summary>
		/// <param name="task">The task to observe.</param>
		/// <param name="timeout">Time to wait for the task to complete (in ms).</param>
		public static async Task DoesNotCompleteAsync(Task task, int timeout = 500)
		{
			// wait for the task to complete or the timeout to elapse
			var completedTask = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);
			
			// abort, if the specified task completed before the timeout task
			if (completedTask == task) {
				throw new Exception("Task completed unexpectedly.");
			}

			// if the task didn't complete, attach a continuation that will raise an exception
			// on a random thread pool thread, if it ever does complete.
			try
			{
				throw new Exception("Task completed unexpectedly.");
			}
			catch (Exception ex)
			{
				var info = ExceptionDispatchInfo.Capture(ex);
				var __ = task.ContinueWith(_ => info.Throw(), TaskScheduler.Default);
			}
		}
	}
}
