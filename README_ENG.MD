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

The asynchronous method, like the normal method, can use any number of parameters or no parameters. However, an asynchronous method cannot define parameters using the **out**, **ref**, and **in** modifiers.

It is also worth noting that the word **async**, which is included in the definition of a method, does NOT automatically make it an asynchronous method. It merely indicates that the method may contain one or more statements An asynchronous method, like a normal method, may use any number of parameters or no parameters. However, an asynchronous method cannot define parameters using the out, ref, and in modifiers.

It is also worth noting that the word asynchronous, which appears in the definition of a method, does NOT automatically make it an asynchronous method. It merely indicates that the method may contain one or more **await** statements.

Consider the simplest example of defining and calling an asynchronous method:.
```C#
await PrintAsync(); // calling the asynchronous method
Console.WriteLine("Some actions in the Main method");
 
void Print()
{
    Thread.Sleep(3000); // simulate continuous operation
    Console.WriteLine("Hello WORLD AND STUDENTS");
}
 
// asynchronous method definition
async Task PrintAsync()
{
    Console.WriteLine("Begin PrintAsync method"); // runs synchronously
    await Task.Run(() => Print()); // asynchronously
    Console.WriteLine("End of PrintAsync method");
}   
```

Here the usual Print method is defined first, which simply prints a string to the console. A three-second delay is used to simulate a long operation using the Thread.Sleep() method. So, conventionally, Print is some method that performs some long operation. In a real application, this might be accessing a database or reading and writing a file, but to simplify understanding, it simply prints some string to the console.

This is also where the asynchronous PrintAsync() method is defined. It is asynchronous because it has the **async** modifier before the return type in its definition, its return type is Task, and the **await** expression is defined in the method body.

It should be noted that the PrintAsync method does not explicitly return any Task object, but since the **await** expression is used in the method body, the Task type can be used as the return type.

Let's see what the console output of the program will be:
```
Start of the PrintAsync method
Hello WORLD AND STUDENTS
End of PrintAsync method
Some actions in the Main method
```
1. The program is started, more precisely the Main method in which the asynchronous PrintAsync method is called.
2. The PrintAsync method starts executing synchronously after the await expression.
3. The await expression starts the asynchronous task Task.Run((()=>Print()))).
4. While the asynchronous task Task.Run((()=>Print())) is running (and can run for quite a long time), the code execution returns to the calling method - that is, the Main method.
5. Once the asynchronous task finishes executing (in the above example, it printed a line after three seconds), the PrintAsync method that called the asynchronous task continues to run.
6. After the PrintAsync method completes, the Main method continues to run.

### Asynchronous method Main
Note that the **await** operator can only be used in a method that has the **async** modifier. And if the **await** operator is used in the Main method, the Main method must also be defined as asynchronous. This means that the previous example will actually be similar to the following one:
```C#
class Program
{
    async static Task Main(string[] args)
    {
        await PrintAsync(); // call asynchronous method
        Console.WriteLine("Some actions in the Main method");
 
 
        void Print()
        {
            Thread.Sleep(3000); // simulate continuous operation
            Console.WriteLine("Hello WORLD AND STUDENTS");
        }
 
        // asynchronous method definition
        async Task PrintAsync()
        {
            Console.WriteLine("Begin PrintAsync method"); // executed synchronously
            await Task.Run(() => Print()); // executed asynchronously
            Console.WriteLine("End of PrintAsync method");
        }
    }
}
```

### Asynchronous operation delay and Task.Delay
In asynchronous methods, the **Task.Delay()** method can be used to stop the method for a certain period of time. It accepts as a parameter the number of milliseconds as an int value or a TimeSpan object that specifies the delay time:
```C#
await PrintAsync(); // calling the asynchronous method
Console.WriteLine("Some actions in the Main method");
 
// asynchronous method definition
async Task PrintAsync()
{
    await Task.Delay(3000); // simulate continuous operation
    // or something like that
    //await Task.Delay(TimeSpan.FromMilliseconds(3000));
    Console.WriteLine("Hello WORLD AND STUDENTS");
}   
```
Moreover, the Task.Delay method is itself an asynchronous operation, so the await operator is applied to it.

### Advantages of asynchronicity
The above examples are simplified and can hardly be considered illustrative. Let us consider another example:
```C#
PrintName("Tom");
PrintName("Bob");
PrintName("Sam");
 
void PrintName(string name)
{
    Thread.Sleep(3000);     // 
    Console.WriteLine(name);
}
```
This code is synchronous and executes three calls of the PrintName method in sequence. Since the method has a three-second delay to simulate a long operation, the total execution of the program will take at least 9 seconds. Because each subsequent call to the PrintName method will wait until the previous call has completed.

Let's change the synchronous PrintName method to an asynchronous method:
```C#
await PrintNameAsync("Tom");
await PrintNameAsync("Bob");
await PrintNameAsync("Sam");
 
// asynchronous method definition
async Task PrintNameAsync(string name)
{
    await Task.Delay(3000); // simulate continuous operation
    Console.WriteLine(name);
}
```
Instead of the PrintName method, the PrintNameAsync method is now called three times. To simulate long run times, the method is delayed by 3 seconds by calling Task.Delay(3000). And because each method call uses the await operator to stop execution until the asynchronous method completes, the total program execution will again take at least 9 seconds. However, now the execution of asynchronous operations is not blocked by the main thread.

Now let's optimize the program:
```C#
var tomTask = PrintNameAsync("Tom");
var bobTask = PrintNameAsync("Bob");
var samTask = PrintNameAsync("Sam");
 
await tomTask;
await bobTask;
await samTask;
// asynchronous method definition
async Task PrintNameAsync(string name)
{
    await Task.Delay(3000); // simulate continuous operation
    Console.WriteLine(name);
}
```
In this case, the tasks are actually run on definition. And the await operator is only used when we need to wait for asynchronous operations to complete - that is, at the end of the program. And in this case, the total program execution will take not less than 3 seconds, but much less than 9 seconds.

## Return result from asynchronous method
The return type in the asynchronous method must be **void**, **Task**, **Task<T>** or **ValueTask<T>**.

### void
If the void keyword is used, the asynchronous method returns nothing:
```C#
PrintAsync("Hello World");
PrintAsync("Hello STUDENTS");
 
Console.WriteLine("Main End");
await Task.Delay(3000); // waiting for tasks to complete
 
// asynchronous method definition
async void PrintAsync(string message)
{
    await Task.Delay(1000); // simulate continuous operation
    Console.WriteLine(message);
}   
```
However, asynchronous void methods should be avoided and only used when similar methods are the only possible way to define an asynchronous method. In particular, we cannot use the await operator on such methods. Also because exceptions in such methods are difficult to handle because they cannot be caught outside the method. In addition, such void methods are difficult to test.

Still, there are situations where we cannot do without such methods - for example, in event processing:
```C#
Account account = new Account();
account.Added += PrintAsync;
 
account.Put(500);
 
await Task.Delay(2000); // waiting for completion
 
// asynchronous method definition
async void PrintAsync(object? obj, string message)
{
    await Task.Delay(1000); // simulate continuous operation
    Console.WriteLine(message);
}
 
class Account
{
    int sum = 0;
    public event EventHandler<string>? Added;
    public void Put(int sum)
    {
        this.sum += sum;
        Added?.Invoke(this, $"The account has been credited with {sum} $");
    }
}
```
In this case, the Added event in the Account class is represented by an EventHandler delegate that has type void. Therefore, only a handler method with type void can be defined for this event.

### Task
Returns an object of type Task:
```C#
await PrintAsync("Hello WORLD AND STUDENTS");
 
// asynchronous method definition
async Task PrintAsync(string message)
{
    await Task.Delay(1000); // simulate continuous operation
    Console.WriteLine(message);
}
```
Here, the PrintAsync method does not formally use the return operator to return the result. However, if an asynchronous operation is performed in the await statement of an asynchronous method, we can return a Task object from the method.

If we want to wait for the asynchronous task to complete, we can use the **await** operator. And it is not necessary to use it directly when calling the task. It can only be used where we need to guarantee that we get the result of the task, or to make sure that the task has completed.
```C#
var task = PrintAsync("Hello WORLD AND STUDENTS"); // start the task
Console.WriteLine("Main Works");
 
await task; // wait for the task to complete
 
// asynchronous method definition
async Task PrintAsync(string message)
{
    await Task.Delay(0);
    Console.WriteLine(message);
}
```

### Task < T >
The method can return a value. Then the returned value is wrapped in a Task object and the return type is Task<T>:
```c#
int n1 = await SquareAsync(5);
int n2 = await SquareAsync(6);
Console.WriteLine($"n1={n1} n2={n2}"); // n1=25 n2=36
 
async Task<int> SquareAsync(int n)
{
    await Task.Delay(0);
    return n * n;
}
```
In this case, the Square method returns a value of type int - the square of the number. The return type in this case is of type Task<int>.

To get the result of an asynchronous method, use the await operator when calling SquareAsync:
```
int n1 = await SquareAsync(5);
```
Other data types can be retrieved in a similar way:
```C#
Person person = await GetPersonAsync("Tom");
Console.WriteLine(person.Name); // Tom
// asynchronous method definition
async Task<Person> GetPersonAsync(string name)
{
    await Task.Delay(0);
    return new Person(name);
}
record class Person(string Name);
```

### ValueTask < T >
Using the ValueTask<T> type is very similar to using the Task<T> type, except for some differences in memory handling, since ValueTask is a structure that contains multiple fields. Therefore, using a ValueTask instead of a Task results in more data being copied and, as a result, creates some additional overhead.

The advantage of ValueTask over Task is that this type avoids the additional memory allocation in the chip. For example, sometimes you need to return a value synchronously. Consider the following example:
```C#
var result = await AddAsync(4, 5);
Console.WriteLine(result);
 
Task<int> AddAsync(int a, int b)
{
    return Task.FromResult(a + b);
}
```
Here the AddAsync method synchronously returns a value - in this case the sum of two numbers. You can use the static Task.FromResult method to synchronously return a value. However, using the Task type will result in the allocation of another task with an associated memory allocation in the hip. ValueTask solves this problem:
```C#
var result = await AddAsync(4, 5);
Console.WriteLine(result);
 
ValueTask<int> AddAsync(int a, int b)
{
    return new ValueTask<int>(a + b);
}
```
In this case, no additional task object will be created and therefore no additional memory will be allocated. Therefore, ValueTask is usually used in cases where the result is 
