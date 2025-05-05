# LINQ
### 1. **Where**: Filters elements based on a condition.

```c#
var activeItems = _context.Items
    .Where(i => i.IsActive)
    .ToList();
```

This retrieves all items from the database where `IsActive` is `true`.

```c#
var filteredResult = studentList.Where(s => s.Age > 12 && s.Age < 20);
```

---
### 2. **FirstOrDefault**: Returns the first element of a collection, or the first element that satisfies a condition. Returns a default value if index is out of range.

```c#
var category = _context.Categories
    .FirstOrDefault(c => c.Name == "Beverages");
```
This gets the first category with the name "Beverages" or returns `null` if no such category exists.
```c#
IList<int> intList = new List<int>() { 7, 10, 21, 30, 45, 50, 87 };
IList<string> strList = new List<string>() { null, "Two", "Three", "Four", "Five" };
IList<string> emptyList = new List<string>();
		
Console.WriteLine("1st Element in intList: {0}", intList.FirstOrDefault());
Console.WriteLine("1st Even Element in intList: {0}",
                                 intList.FirstOrDefault(i => i % 2 == 0));

Console.WriteLine("1st Element in strList: {0}", strList.FirstOrDefault());

Console.WriteLine("1st Element in emptyList: {0}", emptyList.FirstOrDefault());
```

---
### 3. **Find**: Retrieves an entity by its primary key. This method is part of EF Core's `DbSet`.
```c#
var item = _context.Items.Find(1);
```
This retrieves the item with the primary key of `1`. It is efficient because it first checks the local context for the entity before querying the database.

---
### 4. **Include**: Specifies related entities to be included in the query results (eager loading).

```c#
var orderWithDetails = _context.Pedidos
    .Include(p => p.DetalhePedidos)
    .FirstOrDefault(p => p.Id == 1);
```

This retrieves an order with its associated `DetalhePedidos` based on the order ID.

---
### 5. **OrderBy**: Sorts the results in ascending order based on a specified field.

```c#
var orderedItems = _context.Items
    .OrderBy(i => i.Name)
    .ToList();
```

```c#
 IList<Student> studentList = new List<Student>() { 
    new Student() { StudentID = 1, StudentName = "John", Age = 18 } ,
    new Student() { StudentID = 2, StudentName = "Steve",  Age = 15 } ,
    new Student() { StudentID = 3, StudentName = "Bill",  Age = 25 } ,
    new Student() { StudentID = 4, StudentName = "Ram" , Age = 20 } ,
    new Student() { StudentID = 5, StudentName = "Ron" , Age = 19 } 
};

var studentsInAscOrder = studentList.OrderBy(s => s.StudentName);
```

---
### 6. **ThenBy**: Further orders the result of a previous `OrderBy`.

```c#
var orderedItems = _context.Items
    .OrderBy(i => i.CategoryId)
    .ThenBy(i => i.Price)
    .ToList();
```

This first orders the items by `CategoryId` and then by `Price` within each category.

```c#
IList<Student> studentList = new List<Student>() { 
    new Student() { StudentID = 1, StudentName = "John", Age = 18 } ,
    new Student() { StudentID = 2, StudentName = "Steve",  Age = 15 } ,
    new Student() { StudentID = 3, StudentName = "Bill",  Age = 25 } ,
    new Student() { StudentID = 4, StudentName = "Ram" , Age = 20 } ,
    new Student() { StudentID = 5, StudentName = "Ron" , Age = 19 }, 
    new Student() { StudentID = 6, StudentName = "Ram" , Age = 18 }
};
var thenByResult = studentList.OrderBy(s => s.StudentName).ThenBy(s => s.Age);

var thenByDescResult = studentList.OrderBy(s => s.StudentName).ThenByDescending(s => s.Age);
```

---
### 7. **Any**: Checks if any elements match the condition.

```c#
bool hasActiveItems = _context.Items.Any(i => i.IsActive);
```

---
### 8. **Count**: Returns the number of elements that match a condition.

```c#
int activeItemsCount = _context.Items.Count(i => i.IsActive);
```

This retrieves the count of active items.

```c#
IList<int> intList = new List<int>() { 10, 21, 30, 45, 50 };

var totalElements = intList.Count();

Console.WriteLine("Total Elements: {0}", totalElements);

var evenElements = intList.Count(i => i%2 == 0);

Console.WriteLine("Even Elements: {0}", evenElements);
```
```c# 
IList<Student> studentList = new List<Student>>() { 
        new Student() { StudentID = 1, StudentName = "John", Age = 13} ,
        new Student() { StudentID = 2, StudentName = "Moin",  Age = 21 } ,
        new Student() { StudentID = 3, StudentName = "Bill",  Age = 18 } ,
        new Student() { StudentID = 4, StudentName = "Ram" , Age = 20} ,
        new Student() { StudentID = 5, StudentName = "Mathew" , Age = 15 } 
    };

var totalStudents = studentList.Count();

Console.WriteLine("Total Students: {0}", totalStudents);

var adultStudents = studentList.Count(s => s.Age >= 18);

Console.WriteLine("Number of Adult Students: {0}", adultStudents );
```

---
### 9 Projection
There are two projection operators available in LINQ. 1) Select 2) SelectMany
### 9.1: **Select**
The Select operator always returns an IEnumerable collection which contains elements based on a transformation function. It is similar to the Select clause of SQL that produces a flat result set.

```c#
var itemNames = _context.Items
    .Where(i => i.IsActive)
    .Select(i => i.Name)
    .ToList();
```

This retrieves a list of the names of all active items.

```c#
IList<Student> studentList = new List<Student>() { 
    new Student() { StudentID = 1, StudentName = "John" },
    new Student() { StudentID = 2, StudentName = "Moin" },
    new Student() { StudentID = 3, StudentName = "Bill" },
    new Student() { StudentID = 4, StudentName = "Ram" },
    new Student() { StudentID = 5, StudentName = "Ron" } 
};

var selectResult = from s in studentList
                   select s.StudentName;
```

## 9.2 SelectMany
The SelectMany operator projects sequences of values that are based on a transform function and then flattens them into one sequence.

---
### 10. **GroupBy**: Groups elements that share a common attribute.

```c#
var itemsGroupedByCategory = _context.Items
    .GroupBy(i => i.CategoryId)
    .ToList();
```
This groups the items by their `CategoryId`.

---
### 11. **SingleOrDefault**: Returns the only element that satisfies a condition or the default value if no elements or more than one element exist.
```c#
var category = _context.Categories
    .SingleOrDefault(c => c.Id == 1);
```

This returns the category with an `Id` of 1 or `null` if it does not exist. It throws an exception if more than one element matches the condition.

---
### 12. **Skip and Take**: Used for pagination.

```c#
var paginatedItems = _context.Items
    .OrderBy(i => i.Name)
    .Skip(10)
    .Take(10)
    .ToList();
```
This skips the first 10 items and takes the next 10.

---
### 13. **Join**
### 13.1: Join
The Join operator joins two sequences (collections) based on a key and returns a resulted sequence.
```c#
var itemDetails = _context.Items
    .Join(_context.Categories,
          item => item.CategoryId,
          category => category.Id,
          (item, category) => new { item.Name, category.Name })
    .ToList();
```
This performs an inner join between the `Items` and `Categories` and selects the item name and category name.

13.2: GroupJoin 

---
### 14. **ToList**: Executes the query and returns the results as a list.

```c#
var allItems = _context.Items.ToList();
```
This retrieves all items as a list.

---

# EF CLI

```bash
dotnet ef migrations add NomeDaMigration
```
Adds a new migration.

---

```bash
dotnet ef migrations remove
```
Removes the last migration, rolling back the code changes that were done for the latest migration.

| Option    | Short | Description                                                                                                                                                                                                         |
| --------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `--force` | `-f`  | Revert the latest migration, rolling back both code and database changes that were done for the latest migration. Continues to roll back only the code changes if an error occurs while connecting to the database. |

---

```bash
dotnet ef migrations has-pending-model-changes
```
Checks if any changes have been made to the model since the last migration.

---

```bash
dotnet ef migrations list
```
Lists available migrations.

| Option                      | Description                                                                                            |
| --------------------------- | ------------------------------------------------------------------------------------------------------ |
| `--connection <CONNECTION>` | The connection string to the database. Defaults to the one specified in AddDbContext or OnConfiguring. |

---
## Database
```bash
Deletes the database.
```

| Option      | Short | Description                                              |
| ----------- | ----- | -------------------------------------------------------- |
| `--force`   | `-f`  | Don't confirm.                                           |
| `--dry-run` |       | Show which database would be dropped, but don't drop it. |

```bash
dotnet ef database update
```
Updates the database to the last migration or to a specified migration.

| Argument      | Description                                                                                                                                                                                                                                                     |
| ------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `<MIGRATION>` | The target migration. Migrations may be identified by name or by ID. The number 0 is a special case that means _before the first migration_ and causes all migrations to be reverted. If no migration is specified, the command defaults to the last migration. |
## Migrations

```bash
dotnet ef migrations add NomeDaMigration
```
Adds a new migration.

---

```bash
dotnet ef migrations remove
```
Removes the last migration, rolling back the code changes that were done for the latest migration.

| Option    | Short | Description                                                                                                                                                                                                         |
| --------- | ----- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `--force` | `-f`  | Revert the latest migration, rolling back both code and database changes that were done for the latest migration. Continues to roll back only the code changes if an error occurs while connecting to the database. |

---

```bash
dotnet ef migrations has-pending-model-changes
```
Checks if any changes have been made to the model since the last migration.

---

```bash
dotnet ef migrations list
```
Lists available migrations.

| Option                      | Description                                                                                            |
| --------------------------- | ------------------------------------------------------------------------------------------------------ |
| `--connection <CONNECTION>` | The connection string to the database. Defaults to the one specified in AddDbContext or OnConfiguring. |

---

