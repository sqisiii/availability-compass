# Architecture Decision Record: Choosing an Architecture Pattern

## Status

Accepted

# Context

I am developing a search application that gathers data from various sources, such as trip companies. It should allow
users to define periods when they can or cannot go. This information will be used in the search functionality.
I’ve considered two architectural patterns **Clean Architecture** and **Vertical Slice Architecture**.

They approach structuring code and managing dependencies from different perspectives. Let’s break them down:

The core idea of **Clean Architecture** is to organize the application around layers with clear separation of concerns,
emphasizing independence from frameworks, UI, databases, and external agencies. This approach is great for complex
applications where business rules are complex and expected to change.

In contrast to layering the whole system horizontally (UI layer, application layer, domain layer, etc.), **Vertical
Slice Architecture** structures the code by **features** or **use cases**. Each feature is a **self-contained** slice
that cuts through all layers needed to fulfill a specific functionality.

After carefully evaluating the requirements and constraints of my application, I have decided to use a Vertical Slice
Architecture.

# Rationale

I have chosen to implement a Vertical Slice Architecture for this project for the following reasons:

1. Diversifying Architectural Approaches: My recent experience has been heavily focused on projects structured around
   Clean Architecture. While effective, Clean Architecture can introduce a lot of ceremony and boilerplate, particularly
   in layered systems. I believe it is important to explore alternative patterns like Vertical Slice Architecture to
   broaden my toolkit and improve my delivery speed in certain types of applications.

2. Improving Developer Efficiency in Complex Systems: As projects grow in complexity, adding new functionality under a
   layered architecture often requires touching multiple, sometimes unrelated, parts of the codebase (e.g., domain
   models, services, repositories, controllers). This leads to fragmented change sets and increased cognitive load.
   Vertical Slice Architecture addresses this by encouraging feature-centric design, where all elements for a use case
   are colocated within a single slice. This simplifies the process of implementing new functionality and reduces the
   risk of inadvertently impacting unrelated areas of the system.

Given that this is a desktop application with multiple layers and a growing number of features, Vertical Slice
Architecture offers a more streamlined approach. It will make onboarding easier, reduce friction when evolving the
system, and allow for faster iteration cycles — all while maintaining a clear structure and separation of concerns
within each slice.

# Consequences

Adopting Vertical Slice Architecture will have the following consequences:

1. Feature Isolation: Each feature or use case will be self-contained, encapsulating all necessary components (handlers,
   models, validations, data access) within a single slice. This will make the system easier to extend and maintain,
   especially as the application scales.
2. Reduced Layer Coupling: By moving away from traditional layered dependencies, I reduce the need for global services,
   repositories, or domain-wide abstractions. While this improves modularity, it may also result in some duplication
   across slices where shared logic is minimal or highly specific.
3. Shift in Design Mindset: I will need to adjust from a layered, system-wide mindset (e.g., Service Layer, Repository
   Layer) to a feature-oriented mindset. This may require some initial learning curve and a shift in how I approach code
   organization.
4. Alignment with CQRS Patterns: Vertical Slice Architecture naturally aligns with Command Query Responsibility
   Segregation (CQRS). While this is beneficial for separating read and write concerns, it also means we’ll need to be
   disciplined about maintaining consistency and avoiding premature abstraction.
5. Potential Tooling Adjustments: Some of my current tooling, templates, or conventions may assume a layered structure.
   These will need to be revisited or adapted to support the slice-based organization.

Overall, the choice of Vertical Slice Architecture is expected to lead to faster feature delivery, clearer separation of
responsibilities, and a more maintainable codebase as the project evolves.