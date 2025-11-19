# Kanban Demo - Quick Start Guide

## Project Location
`/mnt/c/src/Core/samples/LionFire.Blazor.Components.MudBlazor.Experimental`

## Key Files

| File | Purpose |
|------|---------|
| `Components/Pages/KanbanDemo.razor` | Main demo page (23KB) |
| `Components/Layout/NavMenu.razor` | Updated with Kanban link |
| `Components/Pages/Home.razor` | Enhanced home page with feature cards |
| `Program.cs` | Configured MudBlazor services |
| `Components/_Imports.razor` | Added MudBlazor imports |
| `Components/Kanban/_Imports.razor` | NEW - Kanban component imports |

## Running the Application

```bash
# Navigate to project directory
cd /mnt/c/src/Core/samples/LionFire.Blazor.Components.MudBlazor.Experimental

# Build the project
dotnet_win build

# Run the application
dotnet_win run

# Access the demo
# Home: http://localhost:5000/
# Kanban Demo: http://localhost:5000/kanban-demo
```

## Application Structure

```
KanbanDemo.razor
├── Header (Title + Description)
├── Control Buttons
│  ├── Board Settings - Opens board configuration dialog
│  ├── Add Card - Creates new card in first column
│  ├── Add Column - Creates new workflow column
│  └── Reset Demo - Restores initial sample data
├── Statistics Grid
│  ├── Total Columns Count
│  ├── Total Cards Count
│  ├── Approved Columns Count
│  └── Board Status
├── Kanban Board Container
│  ├── Column 1: "To Do" (Red) - 10 cards max
│  ├── Column 2: "In Progress" (Cyan) - 5 cards max
│  ├── Column 3: "Review" (Yellow) - 3 cards max
│  └── Column 4: "Done" (Green) - Unlimited
└── Activity Log (Last 10 actions)
```

## Sample Data Included

### Columns (4 total)
- **To Do** (FF6B6B - Red)
  - Status: Approved
  - WIP Limit: 10
  - Sample cards: 3

- **In Progress** (4ECDC4 - Cyan)
  - Status: Approved
  - WIP Limit: 5
  - Sample cards: 2

- **Review** (FFD93D - Yellow)
  - Status: Conditional (Needs approval per task)
  - WIP Limit: 3
  - Sample cards: 1

- **Done** (6BCB77 - Green)
  - Status: Not Approved (Archive)
  - WIP Limit: Unlimited
  - Sample cards: 2

### Sample Cards (8 total)
Each card includes:
- Title and description
- Priority (Low/Medium/High/Critical)
- State (Available/Claimed/InProgress/Blocked/Complete)
- Tags (infrastructure, backend, ui, etc.)
- Optional: assigned to, claimed by, due date, estimates, progress %

## Interactive Features

### Card Operations
1. **Drag & Drop**: Drag any card to a different column
2. **Claim/Release**: Click card menu → Claim to assign to agent
3. **Edit**: Click card menu → Edit to open card dialog
4. **Delete**: Click card menu → Delete to remove card
5. **View Details**: Click card to see full information

### Column Operations
1. **Add Column**: Click "Add Column" button
2. **View Settings**: Each column shows:
   - Name and description
   - Current cards / WIP limit
   - Greenlight status icon (green/yellow/red)

### Board Operations
1. **Board Settings**: Click "Board Settings" to configure:
   - Board name
   - Description
   - Theme (default, dark, light, colorful)
   - Active status
   - Advanced JSON settings

2. **Reset Board**: Click "Reset Demo" to:
   - Clear all changes
   - Restore original 4 columns
   - Reload 8 sample cards
   - Clear activity log

## Greenlight System

### Status Indicators
- **Green (Approved)**: Agents can work autonomously
  - Shown with checkmark icon
  - Example: "To Do" and "In Progress" columns

- **Yellow (Conditional)**: Agents need permission per task
  - Shown with pause/clock icon
  - Example: "Review" column (needs human approval)

- **Red (Not Approved)**: Agents cannot work on these tasks
  - Shown with X icon
  - Example: "Done" column (archive only)

## Activity Log

The activity log at the bottom tracks:
- Card movements between columns
- Card creation/deletion
- Card edits
- Card claims/releases
- Board updates
- Column creation

Each log entry shows:
- Action icon (color-coded)
- Description
- Timestamp (HH:mm:ss format)

## Dialog Features

### Board Dialog
- Edit board name and description
- Change theme
- Toggle active status
- Configure archived status
- View board statistics (columns, total cards, approved count)
- Advanced JSON settings support

### Card Dialog
- Title and description
- Priority selection
- State selection
- Assignment tracking
- Due date picker
- Time estimates (estimated/actual hours)
- Progress slider (0-100%)
- Current step description
- Tags (comma-separated)
- External system integration (GitHub, GitLab, etc.)
- Claim tracking (shows agent claim status)

### Column Dialog
- Column name and description
- Color picker (hex color code)
- Column type (Priority/Status/Custom)
- Greenlight status selection
- Greenlight reason (when not approved)
- WIP limit configuration
- Visibility toggle

## Component Relationships

```
KanbanDemo.razor
│
├── KanbanColumn.razor (x4 columns)
│   ├── KanbanCardComponent.razor (x8 cards total)
│   └── MudDropZone (for drag-drop)
│
├── BoardDialog.razor (modal)
├── CardDialog.razor (modal)
└── ColumnDialog.razor (modal)
```

## State Management

The demo uses in-memory state for immediate updates:
- No backend API calls
- Real-time UI updates
- State persists until page refresh or reset

For production use, consider:
- Connect to Entity Framework DbContext
- Implement API endpoints
- Add user authentication
- Use SignalR for real-time multi-user updates

## Styling Classes

### Custom Classes
- `.kanban-board-wrapper` - Main board container with gray background
- `.kanban-scroll` - Horizontal scrolling for columns
- `.kanban-column` - Individual column styling
- `.kanban-card` - Card with hover effects
- `.activity-log` - Activity log container with max-height

### MudBlazor Classes Used
- `pa-*` - Padding (pa-2, pa-3, pa-4)
- `mb-*` - Margin bottom (mb-2, mb-3, mb-4)
- `mt-*` - Margin top (mt-2, mt-3, mt-4)
- `text-center` - Center text alignment
- `d-flex` - Flexbox display
- `gap-*` - Gap between flex items
- `Typo.*` - Typography classes

## Customization Options

### Add More Sample Cards
Edit the `sampleCards` array in the `InitializeBoard()` method

### Change Column Colors
Update the `Color` property when creating columns:
```csharp
Color = "#FF6B6B",  // Change hex color
```

### Adjust WIP Limits
Modify the `WipLimit` property:
```csharp
WipLimit = 10,  // Change to desired limit
```

### Add New Columns
In `InitializeBoard()`, create new `Models.KanbanColumn` and add to `board.Columns`

## Testing Checklist

- [ ] Page loads successfully
- [ ] Drag and drop works between columns
- [ ] Card menus appear on card right-click
- [ ] Create new card via dialog
- [ ] Edit existing card
- [ ] Delete card
- [ ] Claim card to agent
- [ ] Release claimed card
- [ ] Add new column
- [ ] Edit board settings
- [ ] Activity log updates in real-time
- [ ] Reset board restores initial state
- [ ] Greenlight indicators show correctly
- [ ] WIP limits prevent card drops
- [ ] Mobile responsive layout

## Troubleshooting

### Cards not dragging
- Ensure MudDropContainer is properly bound
- Check column IDs match card ColumnId values

### Dialog not opening
- Verify MudDialogService is injected
- Check Dialogs service is configured in Program.cs

### Styling issues
- Clear browser cache
- Verify MudBlazor styles are loaded
- Check CSS class names match

### Missing components
- Ensure Components namespace is in _Imports.razor
- Verify Kanban folder _Imports.razor exists

## Performance Notes

- Sample board with 8 cards and 4 columns renders instantly
- Drag-drop operations are smooth without lag
- Activity log limits to last 10 entries for performance
- Consider pagination for boards with 100+ cards

## Future Enhancements

1. **Persistence**: Save board state to database
2. **Collaboration**: Add SignalR for multi-user real-time updates
3. **Filtering**: Add filters by priority, assignee, tag
4. **Reporting**: Export board to PDF/CSV
5. **Templates**: Create board templates
6. **Search**: Full-text search across cards
7. **Notifications**: Alerts for card assignments
8. **API Integration**: Connect to GitHub/GitLab issues
9. **Mobile App**: Native mobile clients
10. **Analytics**: Board metrics and burndown charts

---

**Demo Access URL**: `http://localhost:5000/kanban-demo`

**Documentation**: `/mnt/c/src/Core/samples/KANBAN_DEMO_SUMMARY.md`
