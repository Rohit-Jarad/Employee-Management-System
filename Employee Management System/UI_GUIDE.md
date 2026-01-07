# UI Structure & Best Practices Guide

## 🎨 Design Philosophy

**Simple, Clean, Professional**
- Use Bootstrap 5 for consistent styling
- Clean layouts with proper spacing
- Clear visual hierarchy
- User-friendly forms and tables
- Professional color scheme

## 📐 UI Structure

### 1. **Common Components**

#### Alert Messages (Success/Error)
```html
@if (TempData["SuccessMessage"] != null)
{
    <div class="alert alert-success alert-dismissible fade show" role="alert">
        @TempData["SuccessMessage"]
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    </div>
}
```

#### Form Validation Messages
```html
<div asp-validation-summary="ModelOnly" class="alert alert-danger"></div>
<span asp-validation-for="FieldName" class="text-danger"></span>
```

#### Buttons
- Primary actions: `btn btn-primary` (Save, Create, Update)
- Secondary actions: `btn btn-secondary` (Cancel, Back)
- Danger actions: `btn btn-danger` (Delete)
- Outline: `btn btn-outline-primary` (Alternative style)

### 2. **Page Layout Structure**

```
┌─────────────────────────────────────┐
│  Navigation Bar (from _Layout)      │
├─────────────────────────────────────┤
│  Page Header                         │
│  (Title + Action Button)            │
├─────────────────────────────────────┤
│  Alert Messages (if any)            │
├─────────────────────────────────────┤
│  Main Content                        │
│  (Table/Form/Card)                  │
└─────────────────────────────────────┘
```

### 3. **Color Scheme**

- **Primary**: Bootstrap Blue (`btn-primary`)
- **Success**: Green (`alert-success`, `btn-success`)
- **Danger**: Red (`alert-danger`, `btn-danger`)
- **Info**: Blue (`alert-info`)
- **Background**: White/Clean (`bg-white`)

## 📋 Best Practices for Razor Views

### 1. **Use Strongly Typed Models**
```csharp
@model Employee_Management_System.Models.ViewModels.EmployeeViewModel
```

### 2. **Always Include Validation Scripts**
```html
@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
}
```

### 3. **Use Tag Helpers (Recommended)**
```html
<!-- ✅ GOOD -->
<form asp-action="Create" method="post">
    <label asp-for="FirstName"></label>
    <input asp-for="FirstName" class="form-control" />
    <span asp-validation-for="FirstName" class="text-danger"></span>
</form>

<!-- ❌ AVOID -->
<form action="/Employee/Create" method="post">
    <label>First Name</label>
    <input name="FirstName" type="text" />
</form>
```

### 4. **Proper Form Structure**
```html
<div class="mb-3">
    <label asp-for="FieldName" class="form-label"></label>
    <input asp-for="FieldName" class="form-control" />
    <span asp-validation-for="FieldName" class="text-danger"></span>
</div>
```

### 5. **Consistent Button Placement**
- Primary action (Save, Create, Update): Left side
- Secondary action (Cancel, Back): Right side or below
- Use `d-grid` for full-width buttons
- Use `d-flex justify-content-between` for side-by-side buttons

### 6. **Table Best Practices**
- Striped rows (`table-striped`)
- Hover effects (`table-hover`)
- Responsive (`table-responsive` wrapper)
- Clear column headers
- Action buttons in last column

### 7. **Card Layout for Forms**
```html
<div class="card shadow-sm">
    <div class="card-header bg-primary text-white">
        <h5 class="mb-0">Card Title</h5>
    </div>
    <div class="card-body">
        <!-- Form content -->
    </div>
</div>
```

## 🎯 Page-Specific Guidelines

### Login Page
- Centered card layout
- Simple, focused form
- Clear call-to-action button
- Link to register

### Employee List Page
- Table with all employees
- Search/Filter (optional)
- "Add New" button prominently placed
- Action buttons (View, Edit, Delete) per row
- Empty state message

### Create/Edit Form
- Card layout
- Grouped fields (Personal Info, Contact Info, etc.)
- Required field indicators (*)
- Validation error display
- Submit and Cancel buttons

### Details Page
- Read-only card layout
- Well-organized sections
- Action buttons (Edit, Delete, Back)
- Proper formatting (dates, currency)

### Delete Confirmation
- Warning message
- Display employee info
- Confirmation question
- Delete and Cancel buttons

## 💡 UI Tips

### Spacing
- Use Bootstrap spacing utilities: `mb-3`, `mt-4`, `p-3`
- Consistent margins between sections

### Typography
- Use proper heading hierarchy: `h1`, `h2`, `h3`, etc.
- Consistent font sizes

### Forms
- Group related fields
- Use proper input types (`email`, `tel`, `date`)
- Add placeholders for guidance
- Show required field indicators

### Tables
- Limit columns to essential information
- Make action buttons clearly visible
- Use icons or text for actions

### Responsive Design
- Use Bootstrap grid system
- Test on mobile devices
- Use responsive utilities (`d-md-none`, `d-sm-block`)

## 🔧 Common Bootstrap Classes

### Containers & Layout
- `container` - Centered container
- `container-fluid` - Full-width container
- `row` - Row for grid
- `col-md-6` - Column (50% width on medium screens)

### Cards
- `card` - Card container
- `card-header` - Card header
- `card-body` - Card body
- `card-footer` - Card footer
- `shadow-sm` - Subtle shadow

### Forms
- `form-label` - Label styling
- `form-control` - Input styling
- `form-check` - Checkbox/radio container
- `form-select` - Select dropdown styling

### Buttons
- `btn btn-primary` - Primary button
- `btn btn-secondary` - Secondary button
- `btn btn-danger` - Danger button
- `btn btn-sm` - Small button
- `btn btn-lg` - Large button
- `d-grid` - Full-width button container

### Alerts
- `alert alert-success` - Success message
- `alert alert-danger` - Error message
- `alert alert-info` - Info message
- `alert-dismissible` - Dismissible alert

### Tables
- `table` - Base table
- `table-striped` - Striped rows
- `table-hover` - Hover effect
- `table-responsive` - Responsive wrapper

## ✅ Checklist for Each View

- [ ] Proper model binding (`@model`)
- [ ] Page title set (`ViewData["Title"]`)
- [ ] Alert messages for TempData
- [ ] Validation error display
- [ ] Responsive design
- [ ] Consistent styling with Bootstrap
- [ ] Validation scripts included
- [ ] Proper form structure
- [ ] Accessibility (labels, ARIA attributes)
- [ ] Clear call-to-action buttons
