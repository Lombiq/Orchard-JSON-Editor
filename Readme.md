# Lombiq JSON Editor



## About

Module for displaying a JSON Editor like on [jsoneditoronline.org](https://jsoneditoronline.org/).


## Documentation

You can use the JSON editor either as a content field by adding a _Json Field_ to your content type, or by invoking "JsonEditor" the shape with the below tag helper:

```html
<json-editor 
    content="@viewModel.SomeObjectOrJToken"
    options="@viewModel.JsonEditorOptions"
    name="@Html.NameFor(model => model.SomeObjectOrJToken)"></json-editor>
```
or
```html
<json-editor 
    json="@viewModel.SerializedJsonString"
    options="@viewModel.JsonEditorOptions"
    name="@Html.NameFor(model => model.SerializedJsonString)"></json-editor>
```

The properties are:
- Content ("content"): An `object` or `JToken` that will be serialized via [Newtonsoft Json.Net](https://www.newtonsoft.com/json).
- SerializedJson ("json"): An already serialized `string` that can be used as an _alternative_ to Content. Both shouldn't be used, but in case they are, Content takes precendence.
- Options ("options"): A `JsonEditorOptions` object that contains configuration for the JSON editor. See the XML documentation of the `JsonEditorOptions` for more information.
- InputName ("name"): The HTML name of the associated hidden `<input>` element.

All attributes are optional. If neither content nor json is set, an empty object is taken as the content.


## Contributing and support

Bug reports, feature requests, comments, questions, code contributions, and love letters are warmly welcome, please do so via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
