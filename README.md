# SitecoreItemBuilder
Create items within sitecore with default components and renderings. Helps build templates pages using components that can then be customized. Useful when you have to allow for flexibility in components for a template but also need a default configuration of components on the page to start with. This goes a step further from branch templates by also allowing default renderings being added.

## Configuration File
Configurations managed in a single config file - App_Config\Include\z.OneNorth.CreateCustomItem.config. Details below

### Identifying the template to trigger the process
**CustomItem** - 
Configure the template that you need to trigger the process for
- **templateId**: Template for which the item builder should be triggered
- **deviceId**: Device Id for which renderings should be created for
- **layoutId**: Layout to be used for the presentation

### Configuring components to be created for the item
**Component** - Creating components below the item identified
- **templateId**: Template to use to create the component
- **itemName**: Name to give the sitecore item created for the component
- **displayName**: Display Name for item created
- **sortOrder**: Sortorder
- **Fields**: in the format {'Field':'Value','Field':'Value'} to fill in a default value for fields within the item created.

Components can be nested within one another. Items will be created as sub-items when they are nested

### Setting up Renderings for the components created
**Rendering**
- **datasource**: Path of the component mentioned above or empty. Use relative path from the components above.
- **placeholder**: This depends on your layout selected and the placeholders you have configured there and in other components. In the sample config file here, "maincontent/components" is in the layout and other components inserted within them have "tabs", "tab" and "section". For any placeholders than can hold many different components, use the format as "placeholdername-{0}-0"
- **id**: Rendering to use



