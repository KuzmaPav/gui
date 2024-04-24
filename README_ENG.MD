# C# - Asynchronous programming with WPF-ASYNC
## Asynchronous programming in C#
**CZE Prezentace** https://www.canva.com/design/DAGDQIW3l9A/oUIUeN-_v6rqmi90P29Nrw/edit?utm_content=DAGDQIW3l9A&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton

**ENG Presentation** https://www.canva.com/design/DAGDQefAo5M/oInU8AKs4Um8092NbG9MNQ/edit?utm_content=DAGDQefAo5M&utm_campaign=designshare&utm_medium=link2&utm_source=sharebutton
### Asynchronous methods, async and await
**Asynchrony** allows you to move individual tasks from the main thread to special asynchronous methods and use threads more sparingly. Asynchronous methods are executed in separate threads. However, after performing a long operation, the asynchronous method thread returns to the thread pool and is used for other tasks. And when the long operation completes its execution, a thread from the thread pool is reassigned to the asynchronous method, and the asynchronous method continues its work.

The key to working with asynchronous calls in C# is two operators, **async** and **await**, which aim to simplify writing asynchronous code. They are used together to create an asynchronous method.

The **asynchronous method** has the following properties:
* The async modifier is used in the method header.
* The method contains one or more await expressions.
* One of the following is used as the return type:
   * void
   * Task
   * Task<T>
   * ValueTask<T>
