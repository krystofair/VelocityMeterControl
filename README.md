# Velocity Meter Control


## How to use.

1. You have to prepare images of square shape or using these in this repo. Your images should have equal size to each other, and pointer image has to be transparent with object visible on started position. There will be optimization which delete this alpha pixels and left only object. The same rule refers to extra(middle) images.

2. Load those images as resources and named their as base, middle and pointer. The base file actually will be named as "\_base" in order to not collide with C# keyword. If you don't do this, there might be show an error.

3. Load this project to your "Windows Form App" Project
    1. Click the solution for context menu,
    2. Choose "Add Existing Project",
    3. Find ".csproj" file of this and click "Open".

4. Rebuild solution and check if toolbox has that new control.
