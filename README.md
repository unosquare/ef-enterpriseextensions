# ef-enterpriseextensions
[![Build status](https://ci.appveyor.com/api/projects/status/6e5vk7s69ur34nd0?svg=true)](https://ci.appveyor.com/project/geoperez/ef-enterpriseextensions)

Unosquare Labs EntityFramework.EnterpriseExtensions Library contains a set of useful helpers.

NuGet Installation:
-------------------

[![NuGet version](https://badge.fury.io/nu/Unosquare.EntityFramework.EnterpriseExtensions.svg)](https://badge.fury.io/nu/Unosquare.EntityFramework.EnterpriseExtensions)
```
PM> Install-Package Unosquare.EntityFramework.EnterpriseExtensions
```

or

[![NuGet version](https://badge.fury.io/nu/Unosquare.Identity.EntityFramework.EnterpriseExtensions.svg)](https://badge.fury.io/nu/Unosquare.Identity.EntityFramework.EnterpriseExtensions)
```
PM> Install-Package Unosquare.Identity.EntityFramework.EnterpriseExtensions
```

if you are using Identity EF.

Use
---

You need to change your DbContext or IdentityDbContext to BusinessDbContext or IdentityBusinessDbContext. You can attach controllers to your DbContext
in the constructor and they will execute before <code>SaveChanges</code> method. The controllers require to specified a <code>BusinessRuleAttribute</code> to
identify what CRUD action and which Entity types will be processed.