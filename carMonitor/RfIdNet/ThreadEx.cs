
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace RfidNet
{
    /// <summary>
    /// Enumerator for Thread Operation Code
    /// </summary>
    public enum ThreadOpCode
    {
        /// <summary>
        /// The thread is started when it is created.
        /// </summary>
        CREATE_START = 0,
        /// <summary>
        /// The thread is suspended when it is created.
        /// </summary>
        CREATE_SUSPEND
    };

    /// <summary>
    /// Enumerator for Thread Status
    /// </summary>
    public enum ThreadStatus
    {
        /// <summary>
        /// The thread is started and running.
        /// </summary>
        STARTED = 0,
        /// <summary>
        /// The thread is suspended.
        /// </summary>
        SUSPENDED,
        /// <summary>
        /// The thread is terminated.
        /// </summary>
        TERMINATED
    };

    /// <summary>
    /// Enumerator for Thread Terminate Result
    /// </summary>
    public enum TerminateResult
    {
        /// <summary>
        /// Failed to terminate the thread 
        /// </summary>
        FAILED = 0,
        /// <summary>
        /// The thread terminated gracefully
        /// </summary>
        GRACEFULLY_TERMINATED,
        /// <summary>
        /// The thread terminated forcefully
        /// </summary>
        FORCEFULLY_TERMINATE,
        /// <summary>
        /// The thread was not running
        /// </summary>
        NOT_ON_RUNNING,
    };
    /// <summary>
    /// A class that implements base thread class operation.
    /// </summary>
    public class ThreadEx
    {
        /// <summary>
        /// thread handle 当前线程句柄
        /// </summary>
        private Thread m_threadHandle;
        		
        /// <summary>
        /// ThreadPriority 线程优先级
        /// </summary>
		private ThreadPriority m_threadPriority;
		
        /// <summary>
        /// Parent Thread Handle 父线程句柄
        /// </summary>
		private Thread m_parentThreadHandle;
		
        /// <summary>
        /// Thread Status 线程状态
        /// </summary>
		private ThreadStatus m_status;

        /// <summary>
        /// thread Func 线程函数
        /// </summary>
        private Action m_threadFunc;

        /// <summary>
        /// thread parameterized Func 线程对象
        /// </summary>
        private Action<object> m_threadParameterizedFunc;
        /// <summary>
        /// parameter object for parameterized function  
        /// </summary>
        private object m_parameter;
        /// <summary>
        /// Lock 锁
        /// </summary>
        private Object m_threadLock = new Object();
        /// <summary>
        /// exit code 关闭状态码
        /// </summary>
		private ulong m_exitCode;

        

        /// <summary>
        /// Default Constructor 构造函数
        /// </summary>
        /// <param name="priority">The priority of the thread.</param>
 		public ThreadEx(ThreadPriority priority=ThreadPriority.Normal)
        {
            m_threadHandle=null;
            m_threadPriority=priority;
            m_parentThreadHandle=null;
            m_status=ThreadStatus.TERMINATED;
            m_exitCode=0;
            m_threadFunc = null;
            m_threadParameterizedFunc = null;
            m_parameter = null;
        }

        /// <summary>
        /// Default Constructor 构造函数
        /// </summary>
        /// <param name="threadFunc">the function for the thread</param>
        /// <param name="priority">The priority of the thread.</param>
		public ThreadEx(Action threadFunc, ThreadPriority priority=ThreadPriority.Normal)
        {
            m_threadHandle = null;
            m_threadPriority = priority;
            m_parentThreadHandle = null;
            m_status = ThreadStatus.TERMINATED;
            m_exitCode = 0;
            m_threadFunc = threadFunc;
            m_threadParameterizedFunc = null;
            m_parameter = null;

	        m_parentThreadHandle=Thread.CurrentThread;
	        m_threadHandle=new Thread(ThreadEx.entryPoint);
            m_threadHandle.Priority=m_threadPriority;
            m_threadHandle.Start(this);
            m_status=ThreadStatus.STARTED; 
                  

        }

        /// <summary>
        /// Default Constructor 构造函数
        /// </summary>
        /// <param name="threadParameterizedFunc">the parameterized function for the thread</param>
        /// <param name="priority">The priority of the thread.</param>
        public ThreadEx(Action<object> threadParameterizedFunc,object parameter, ThreadPriority priority = ThreadPriority.Normal)
        {
            m_threadHandle = null;
            m_threadPriority = priority;
            m_parentThreadHandle = null;
            m_status = ThreadStatus.TERMINATED;
            m_exitCode = 0;
            m_threadFunc = null;
            m_threadParameterizedFunc = threadParameterizedFunc;
            m_parameter = parameter;

            m_parentThreadHandle = Thread.CurrentThread;
            m_threadHandle = new Thread(ThreadEx.entryPoint);
            m_threadHandle.Priority = m_threadPriority;
            m_threadHandle.Start(this);
            m_status = ThreadStatus.STARTED;


        }

        /// <summary>
        /// Default copy constructor 默认复制构造函数
        /// </summary>
        /// <param name="b">the object to copy from</param>
        public ThreadEx(ThreadEx b)
        {
            m_threadFunc=b.m_threadFunc;
            m_threadParameterizedFunc = b.m_threadParameterizedFunc;
            m_parameter = b.m_parameter;
	        if(m_threadFunc!=null||m_parentThreadHandle!=null)
	        {
		        m_parentThreadHandle=b.m_parentThreadHandle;
		        m_threadHandle=b.m_threadHandle;
		        m_threadPriority=b.m_threadPriority;
		        m_status=b.m_status;
		        m_exitCode=b.m_exitCode;

		        b.m_parentThreadHandle=null;
		        b.m_threadHandle=null;
		        b.m_status=ThreadStatus.TERMINATED;
		        b.m_exitCode=0;
            }
	        else
	        {
		        m_threadHandle=null;
		        m_threadPriority=b.m_threadPriority;
		        m_parentThreadHandle=null;
		        m_exitCode=0;

                m_status = ThreadStatus.TERMINATED;
	        }
        }

        ~ThreadEx()
        {
            resetThread();
        }
		
        /// <summary>
        /// Start the Thread according to parameters given.
        /// 根据参数启动线程
        /// </summary>
        /// <param name="opCode">The operation code for creating thread.</param>
        /// <param name="stackSize">The stack size for the thread.</param>
        /// <returns>true, if succeeded, otherwise false.</returns>
		public bool Start(ThreadOpCode opCode=ThreadOpCode.CREATE_START, int stackSize=0)
        {
            lock(m_threadLock)
            {
                m_parentThreadHandle=Thread.CurrentThread;
                if(m_status==ThreadStatus.TERMINATED&& m_threadHandle==null)
                {
                    m_threadHandle=new Thread(ThreadEx.entryPoint,stackSize);
                    if (m_threadHandle != null)
                    {
                        m_threadHandle.Priority = m_threadPriority;
                        if (opCode == ThreadOpCode.CREATE_START)
                        {
                            m_threadHandle.Start(this);
                            m_status = ThreadStatus.STARTED;
                        }
                        else
                            m_status = ThreadStatus.SUSPENDED;
                        return true;
                    }

                }
                //	System::OutputDebugString(_T("The thread (%x): Thread already exists!\r\n"),m_threadId);
	                return false;
            }
        }

        /// <summary>
        /// Resume the suspended thread. 恢复线程
        /// </summary>
        /// <returns>true, if succeeded, otherwise false.</returns>
        public bool Resume()
        {
            lock (m_threadLock)
            {
                if (m_status == ThreadStatus.SUSPENDED && m_threadHandle != null)
                {
                    m_threadHandle.Resume();
                    m_status = ThreadStatus.STARTED;
                    return true;
                }
            }
            //	System::OutputDebugString(_T("The thread (%x): Thread must be in suspended state in order to resume!\r\n"),m_threadId);
            return false;
        }

        /// <summary>
        /// Suspend the running thread.  挂起线程
        /// </summary> 
        /// <returns>true, if succeeded, otherwise false.</returns>
        public bool Suspend()
        {

            if(m_status==ThreadStatus.STARTED && m_threadHandle!=null)
            {
                lock(m_threadLock)
                {
                    m_status=ThreadStatus.SUSPENDED;
                }
                m_threadHandle.Suspend();
                return true;
            }
            //	System::OutputDebugString(_T("The thread (%x): Thread must be in running state in order to suspend!\r\n"),m_threadId);
            return false;
            
        }

        /// <summary>
        /// Terminate the running or suspended thread. 强制终止线程
        /// </summary>
        /// <returns>true, if succeeded, otherwise false.</returns>
        public bool Terminate()
        {
            Debug.Assert(m_threadHandle != Thread.CurrentThread, "Exception : Thread should not terminate self.");

            if (m_status != ThreadStatus.TERMINATED && m_threadHandle != null)
            {
                lock (m_threadLock)
                {
                    m_status = ThreadStatus.TERMINATED;
                    m_exitCode = 1;
                    m_threadHandle.Abort();
                    m_threadHandle = null;
                    m_parentThreadHandle = null;
                }
                ulong exitCode = m_exitCode;
                onTerminated(exitCode);
                return true;
            }
            return true;
        }

        /// <summary>
        /// Wait for thread to terminate 等待线程完成
        /// </summary>
        /// <param name="tMilliseconds">the time-out interval, in milliseconds.</param>
        /// <returns>true if successful, otherwise false</returns>
        public bool WaitFor(int tMilliseconds = Timeout.Infinite)
        {
            if(m_status!=ThreadStatus.TERMINATED && m_threadHandle!=null)
	        {
		        return m_threadHandle.Join(tMilliseconds);
	        }
	        else
	        {
                //	System::OutputDebugString(_T("The thread (%x): Thread is not started!\r\n"),m_threadId);
		        return false;
	        }
        }

        /// <summary>
        /// Join the thread 加线入线程
        /// </summary>
        public void Join()
        {
            if (m_status != ThreadStatus.TERMINATED && m_threadHandle != null)
            {
                m_threadHandle.Join();
            }
        }

        /// <summary>
        /// Check if the thread class is joinable 检查当前线程是否可Join
        /// </summary>
        /// <returns>true if joinable otherwise false</returns>
        public bool Joinable()
        {
            return (m_status != ThreadStatus.TERMINATED && m_threadHandle != null);
        }

        /// <summary>
        /// Detach the thread 析构线程
        /// </summary>
        public void Detach()
        {
            Debug.Assert(Joinable() == true);
            lock (m_threadLock)
            {
                m_status = ThreadStatus.TERMINATED;
                m_threadHandle = null;
                m_parentThreadHandle = null;
                m_exitCode = 0;
            }
        }

        /// <summary>
        /// Wait for thread to terminate, and if not terminated, then Terminate. 等待线程结速执行
        /// </summary>
        /// <param name="tMilliseconds">the time-out interval, in milliseconds.</param>
        /// <returns>the terminate result of the thread</returns>
        public TerminateResult TerminateAfter(int tMilliseconds)
        {
           	if(m_status!=ThreadStatus.TERMINATED && m_threadHandle!=null)
	        {
		        bool status=m_threadHandle.Join(tMilliseconds);
                if(status)
                {
                    return TerminateResult.GRACEFULLY_TERMINATED;
                }
                else
                {
                    if (Terminate())
                        return TerminateResult.FORCEFULLY_TERMINATE;
                    return TerminateResult.FAILED;
                }
	        }
	        else
	        {
		        //System::OutputDebugString(_T("The thread (%x): Thread is not started!\r\n"),m_threadId);
		        return TerminateResult.NOT_ON_RUNNING;
	        }
        }

        /// <summary>
        /// Return the parent's Thread Handle. 得到父线程
        /// </summary>
        /// <returns>the parent's Thread Handle.</returns>
		public Thread GetParentThreadHandle()
		{
			return m_parentThreadHandle;
		}

        /// <summary>
        /// Return the Thread Status. 得到当前线程的状态
        /// </summary>
        /// <returns>the current thread status</returns>
		public ThreadStatus GetStatus()
		{
			return m_status;
		}

        /// <summary>
        /// Return the Thread Exit Code. 返回当前线程的关闭状态
        /// </summary>
        /// <returns>the thread exit code.</returns>
        /// <remarks>0 means successful termination, 1 means unsafe termination.</remarks>
		public ulong GetExitCode()
		{
			return m_exitCode;
		}

        /// <summary>
        /// Return the current Thread Priority. 返回当前线程优先级
        /// </summary>
        /// <returns>the current Thread Priority.</returns>
        public ThreadPriority GetPriority()
        {
            return m_threadPriority;
        }

        /// <summary>
        /// Set Priority of the thread 设置当前线程优先级
        /// </summary>
        /// <param name="priority">The priority of the thread</param>
        /// <returns>true if successfully set otherwise false</returns>
        public bool SetPriority(ThreadPriority priority)
        {
            m_threadPriority = priority;
            m_threadHandle.Priority = priority;
            return true;
        }

        /// <summary>
        /// Return the Thread Handle. 返回当前线程的句柄
        /// </summary>
        /// <returns>the current thread handle.</returns>
		protected Thread getHandle()
		{
			return m_threadHandle;
		}


        /// <summary>
        /// Actual Thread Code. 线程的执行体,虚函数
        /// </summary>
        /// <remarks>Subclass should override this function for executing the thread function.</remarks>
        protected virtual void execute()
        {
            if (m_threadFunc != null)
                m_threadFunc();
            else if (m_threadParameterizedFunc != null)
                m_threadParameterizedFunc(m_parameter);
        }

        /// <summary>
        /// Calls when the thread terminated. 当线程关闭后执行
        /// </summary>
        /// <param name="exitCode">the exit code of the thread</param>
        /// <param name="isInDeletion">the flag whether the thread class is in deletion or not</param>
        protected virtual void onTerminated(ulong exitCode, bool isInDeletion = false)
        {
        }

        /// <summary>
        /// Terminate the thread successfully.
        /// 成功关闭
        /// </summary>
        private void successTerminate()
        {
            lock (m_threadLock)
            {
                m_status = ThreadStatus.TERMINATED;
                m_threadHandle = null;
                m_parentThreadHandle = null;
                m_exitCode = 0;
            }
            
            onTerminated(m_exitCode);
        }

        /// <summary>
        /// Running the thread when thread is created. 
        /// </summary>
        /// <returns>the exit code of the current thread.</returns>
        private int run()
        {
            execute();
            successTerminate();
            return 0;
        }
        /// <summary>
        /// Reset Thread 
        /// </summary>
        private void resetThread()
        {
            if(m_status!=ThreadStatus.TERMINATED)
	        {
		        m_exitCode=1;
		        m_threadHandle.Abort();
		        onTerminated(m_exitCode,true);
	        }

	        m_threadHandle=null;
	        m_parentThreadHandle=null;
	        m_exitCode=0;
            m_status = ThreadStatus.TERMINATED;
        }

        /// <summary>
        /// Entry point for the thread 
        /// </summary>
        /// <param name="pThis">The argument for the thread (this for current case)</param>
        private static void entryPoint(object pThis)
        {
            ThreadEx pt = (ThreadEx)pThis;
            pt.run();
        }


    }
}
