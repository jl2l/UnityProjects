using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;

namespace TreeSharpPlus
{
	/// <summary>
	/// This *should* work like LeafInvoke, but take a lambda which
	/// takes a single (integer) argument. Each execution of the Node
	/// calls the lambda with an successive number (i.e. the first
	/// execution is called with 0, the next with 1).
	///
	/// Single instances of Nodes should not be called in parallel.
	/// </summary>
	public class MyNode : LeafInvoke
	{
        protected new Func<int, RunStatus> func_return = null;
        protected new Action<int> func_noReturn = null;
		private int executionCount = 0;

        protected MyNode()
        {
            this.func_noReturn = null;
            this.func_return = null;

            this.term_noReturn = null;
            this.term_return = null;
        }

        public MyNode(
			Func<int,RunStatus> function)
            : this()
        {
            this.func_return = function;
            this.term_return = null;
        }

        public MyNode(
            Action<int> function)
            : this()
        {
            this.func_noReturn = function;
            this.term_return = null;
        }

        public MyNode(
            Func<int,RunStatus> function,
            Action terminate)
            : this()
        {
            this.func_return = function;
            this.term_noReturn = terminate;
        }

        public MyNode(
            Action<int> function,
            Action terminate)
            : this()
        {
            this.func_noReturn = function;
            this.term_noReturn = terminate;
        }

        public MyNode(
            Func<int,RunStatus> function,
            Func<RunStatus> terminate)
            : this()
        {
            this.func_return = function;
            this.term_return = terminate;
        }

        public MyNode(
            Action<int> function,
            Func<RunStatus> terminate)
            : this()
        {
            this.func_noReturn = function;
            this.term_return = terminate;
        }

        public override IEnumerable<RunStatus> Execute()
        {
            if (this.func_return != null)
            {
                RunStatus status = RunStatus.Running;
                while (status == RunStatus.Running)
                {
                    status = this.func_return.Invoke(executionCount);
					executionCount++;
                    if (status != RunStatus.Running)
                        break;
                    yield return status;
                }
                yield return status;
                yield break;
            }
            else if (this.func_noReturn != null)
            {
                this.func_noReturn.Invoke(executionCount);
				executionCount++;
                yield return RunStatus.Success;
                yield break;
            }
            else
            {
                throw new ApplicationException(this + ": No method given");
            }
        }
	}
}
