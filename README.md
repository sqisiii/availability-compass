# Availability Compass

## The Concept

Have you ever tried to find an organized trip for a group of, let’s say, 10 people including family and your friends?

Have you ever tried to prepare the time frames when all of you are available to go and then go through all the popular
trip organizing companies' sites to find the available trips?

Have you ever had to change your finding because someone’s plans changed?

## The Solution

Here comes the **Availability Compass** application, where the user can easily define calendars for each trip member and
then use the data to automatically check your trip provider for trips available in the provided calendar periods.

But
that’s not all. This application doesn’t have to be limited to trip organizing. It can be easily enhanced to check, for
example, theater plays, movies in the cinema, and so on.

## How It Is Done

A calendar (or multiple ones) has to be created for each user. In each calendar, single dates or recurring dates can be
set. Single dates are just one-day entries, while recurring dates can be used to set more than one day of unavailability
which can repeat after a defined number of days a defined number of times. So if your divorced friend has his children
every two weeks, you can make your plans around that. Any family occasions you can’t miss, you can mark this. You have
already planned holidays with your other (better) friends, mark this too.

Each calendar can be of “Only” or “Except” type. With the Only option, you can define the days you are available, while
with the “Except” type you can define when you can’t go but you are free otherwise. You can see the selected days in the
calendar in the middle of the screen.

When you know which trip sources you want to use, then go into the Source section and just refresh the data. It will be
automatically parsed from the website, retrieved by API, or retrieved by the AI agent. There are some examples which use
website parsing. Adding new sources is really easy, just implement the ISourceService interface and mark the
implementing class with an attribute so it can be easily found by the application.

When you have your calendars and sources ready, then just select one or more of the predefined sources and configured
calendars and get the matching results. Too many results? Then you can filter the results even further by providing the
start and end date when you want to go, maybe some text phrase you are looking for, or even one of the filter fields
provided by the integrated source.

In the Results section, you can sort the results any way you want, and there is a link to the trip itself if you want to
book it.

And if you don't want to go blind, there is Dark Theme support too! Just go to Settings page.

## Implementation Details

This is a WPF Vertically Sliced application which follows the MVVM pattern. It uses the CommunityToolkit.MVVM package so
the ViewModels could be used in the future with the MAUI application. It uses the standard MediatR package for pull
communication between slices and Reactive Extensions for .NET (aka Rx.NET or System.Reactive) package for push
communication. 
I know that using those for a small application like this might be like killing a fly by using a bazooka, but it is a developmer showcase application ...
Inside the slices .NET events or simple interfaces for pull and push communication were used as well.

It uses Serilog for structured logging. For the WPF application, the SQLite database was used.

Although Vertical Architecture is used, the slices are separated into two projects. The AvailabilityCompass.Core
contains all view models and business logic while the AvailabilityCompass.WpfClient project contains views and elements
required for the WPF application to work. The idea behind this is that thanks to the CommunityToolkit.MVVM, the
AvailabilityCompass.Core would be used also to create a MAUI client without extensive changes to the View Models and
business logic.

The application uses Dependency Injection. The Microsoft.Extensions.DependencyInjection package is used for the IOC
container.

The UI styling is done with the Material Design In XAML Toolkit.

The application follows SOLID, KISS, DRY, and similar principles handled best to my time constraints.

Architecture Design Records
In the decisions folder, some ADRs created for this solution can be found.