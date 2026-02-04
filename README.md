# [SimpleShop]

> [!NOTE]
> SimpleShop is a small e-commerce backend where users can browse products, add items to a cart, and place orders.  
The goal is to have a realistic but simple application that is easy to extend each week while we add DevOps-related non-functional requirements



## Tech-stack

> [!NOTE]
> Write a short description of your tech-stack here in terms of programming language(s) and database engine(s).

- **Backend:** C# / .NET (ASP.NET Core Web API)
- **Database:** MySQL (relational database)
- **ORM/Data access:** Entity Framework Core (migrations)
- **DevOps/Runtime:** Docker + Docker Compose
- **CI/CD:** GitHub Actions (build, test, analysis, deploy)


## Architecture

> [!NOTE]
> Write a short explanation of your planned architecture here.
- **API layer:** Controllers + DTOs (HTTP endpoints only)
- **Application layer:** Services containing business logic (Products, Cart, Orders, Users)
- **Infrastructure layer:** MySQL + EF Core repositories and migrations



## Feature plan

> [!NOTE]
> For each week below write a short description of the features you plan to build for your project this week.



### Week 5
*Kick-off week - no features to be planned here*

### Week 6
**Feature 1:** Add `Product` model + `GET /api/products` (list all products)

**Feature 2:** Add `POST /api/products` (create a product)

### Week 7
*Winter vacation - nothing planned.*

### Week 8
**Feature 1:** Add `GET /api/products/{id}` (get product by id)  

**Feature 2:** Add `PUT /api/products/{id}` (update a product)

### Week 9
**Feature 1:** Add `DELETE /api/products/{id}` (delete a product)  

**Feature 2:** Add product search: `GET /api/products?search=...`

### Week 10
**Feature 1:** Add `User` model + `POST /api/auth/register`

**Feature 2:** Add `POST /api/auth/login

### Week 11
**Feature 1:** Add `CartItem` model + `POST /api/cart/items` (add item to cart)

**Feature 2:** Add `GET /api/cart` (view cart)

### Week 12
**Feature 1:** Add Update cart item quantity: `PUT /api/cart/items/{id}`

**Feature 2:** Add Remove item from cart: `DELETE /api/cart/items/{id}`

### Week 13
**Feature 1:** Add Create order from cart: `POST /api/orders`

**Feature 2:** Clear cart after successful order 

### Week 14
*Easter vacation - nothing planned.*

### Week 15
**Feature 1:** Add List user orders: `GET /api/orders` 

**Feature 2:** Add Get order details: `GET /api/orders/{id}`

### Week 16
**Feature 1:** Add Cancel an order: `POST /api/orders/{id}/cancel`

**Feature 2:** product stock field and validate stock on order creation

### Week 17
**Feature 1:** Prevent purchasing products with insufficient stock

**Feature 2:** Seed demo data (a few products + one test user)
