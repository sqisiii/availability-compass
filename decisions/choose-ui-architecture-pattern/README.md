# Architecture Decision Record: Choosing a UI Architecture Pattern

## Status

Accepted

# Context

I am developing a search application that gathers data from various sources, such as trip companies. It should allow
users to define periods when they can or cannot go. This information will be used in the search functionality.

I want to create a desktop application, but it is also crucial to consider future expansion plans, including the
potential implementation of a mobile interface through web or mobile applications. To accommodate this, I need a
software architectural pattern that effectively separates the view from the business logic. This separation will ensure
that my application remains modular, maintainable, and scalable across different platforms.

I’ve considered two architectural patterns **Model-View-Controller (MVC)** and **Model-View-ViewModel (MVVM)**.

# Rationale

I would choose MVVM over MVC for the following key reasons:

1. Better separation of concerns, with the business logic in the ViewModel and the View handling only the presentation.

2. Automatic UI updates via data binding, which reduces boilerplate and makes it easier to keep the UI in sync with the
   data.

3. Higher testability, especially for complex logic, as the ViewModel can be unit tested independently of the UI.

4. Greater reusability, as the ViewModel can be reused across different views and even platforms.

5. Better suited for desktop or mobile applications where dynamic, data-driven UIs are common.

For projects that require flexible and maintainable code with a dynamic UI (such as desktop applications built with WPF
or MAUI), MVVM offers a more scalable and efficient architecture than MVC.

# Consequences

Adopting the MVVM architecture will have the following consequences:

1. **Improved Separation of Concerns:**: Implementing MVVM provides separation that makes the ViewModel reusable across
   different views or even platforms, fostering better code reuse and maintainability.

2. **Easier Data Binding and UI Updates**:

   The use of data-binding between the View and ViewModel allows for automatic updates of the UI when the data changes,
   significantly reducing the need for manual UI updates. This leads to fewer bugs and a more dynamic, responsive user
   interface.

   The View only needs to be concerned with presenting data, while the ViewModel handles data manipulation and
   interaction, ensuring the UI is always up to date with minimal effort.

3. **Increased Testability:**

   With MVVM, the ViewModel is decoupled from the View, which makes it testable in isolation. The business logic and
   state management can be unit tested without requiring a UI framework or complex UI interaction.

4. **Cross-Platform Benefits:**

   Given that MVVM is a common pattern in frameworks like WPF and MAUI, adopting it in this project will allow for
   easier cross-platform development. The ViewModel can be reused across different platform-specific views, promoting
   code reuse and reducing duplication of logic.
   This is particularly advantageous if the application is expected to support multiple platforms in the future, as the
   ViewModel and business logic remain consistent across environments.

5. **Limiting Code Behind issues:**

   In MVVM pattern using Code Behind should be greatly limited. It should be used mostly only to change the UI elements
   look or behavior. For example WPF controls expose some of their properties without a possibility to binding a new
   collection to them. To work around this issue it may be required to create additional custom controls or implement
   new behaviors.

# Risks

The are risks related to MVVM and VSA usage in the same project:

1. There can be a duplication of concerns, where the ViewModel (from MVVM) and Feature Handlers (in VSA) overlap in
   responsibilities, especially when handling certain UI-related logic and business rules. This can make the code harder
   to maintain as you may end up with redundant logic or split responsibilities between these two patterns.
2. I could face difficulty in organizing your application code, especially with how to handle data-binding across
   slices. Each slice would likely include its own ViewModel and UI components, which could make it hard to structure
   the application in a way that respects both the slice-based organization and the MVVM principles.
3. Communicating between slices while maintaining the MVVM pattern could lead to a higher-than-expected amount of
   boilerplate code for syncing or passing data between slices, undermining the clean separation and modularity provided
   by VSA.

# Mitigations

To effectively use both MVVM and VSA together, I can use following approaches:

1. **Clear Separation of Responsibilities:** Ensure that ViewModel handles only the UI logic, while Handlers/Commands in
   the VSA slices focus on business logic. Avoid having both handle similar responsibilities. Use something like
   Mediator to handle communication between slices.

2. **Minimal Overlap in State Management:**  Define a clear strategy for managing state, where the ViewModel focuses on
   managing UI-bound state and the slice handlers manage business logic state, possibly using an event-based or
   messaging system to synchronize the two.

3. **Modularize the Codebase**: Follow best practices for organizing and structuring the project to ensure that the
   lines between MVVM and VSA responsibilities remain clear, which will help avoid unnecessary complexity and
   redundancy.
