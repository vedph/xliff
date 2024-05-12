# XLIFF TOOL

This tool is used to help with managing XLIFF files in Angular when using localizable libraries. The main issue here is that Angular provides no way for localizing a single library; it just extracts the messages from all the workspace and merges them into a single file. This tool helps to split the messages into separate files for each library.

## Localization

Here is a procedure to localize libraries in an Angular workspace. This is useful when you have a workspace with multiple libraries and you want to localize each library separately. This way, you can distribute the localization files with the library package and consumers of the library can localize it independently from the rest of the workspace.

**Prerequisite**: 

- `ng add @angular/localize`
- `ng add ng-extract-i18n-merge`

Your libraries should mark localizable text using `i18n` attributes in templates or `$localize` in code.

**Procedure**:

(1) `ng extract-i18n` to extract and merge messages into `src/locale/messages.xlf`. This takes care of merging new entries into the existing messages, but it does not create distinct files for each library. To mark each entry as coming from a specific library, use a conventional description like e.g.:

- `i18n="auth-jwt-admin"` in templates;
- `$localize` with `:description:message` like in this example:

```ts
$localize`:auth-jwt-admin:message`;
```

This way, later you can optionally split the unique file into smaller files and place each in the library package, so consumers of the library will be able to localize them.

(2) copy the files in `locale` to the `src/locale` folder of each library and remove from them all the entries not belonging to that library.

(3) ensure that your library `ng-package.json` includes in its `assets` these XLF files, e.g.:

```json
{
  "$schema": "../../../node_modules/ng-packagr/ng-package.schema.json",
  "assets": ["src/locale/*.xlf"],
  "dest": "../../../dist/myrmidon/auth-jwt-login",
  "lib": {
    "entryFile": "src/public-api.ts"
  }
}
```

Now, when you `ng build` your library the XLF files will be inside its `dist` folder under `locale`.

>💡 To build the app for testing a specific locale: in `angular.json`, under `projects/YOURPROJECT/architect/build/options` add a localize property with the languages you want to build, e.g. `"localize": ["it"]`.

The tool here replaces the manual step (2) above. It reads an `.xlf` file and copies it into an output file where all the units not tagged with the specified description have been removed.

Thus, you can run the tool for each library to create the localized XLF files for it.

## NPM Packaging

To install your npm package locally for testing without having to push every version of it to npm:

1. build your library as usual.
2. pack your library: navigate to the `dist/your-library-name` directory and pack your library using the `npm pack` command. This will create a `.tgz` file.

    ```bash
    cd dist/your-library-name
    npm pack
    ```

3. install your library locally: now you can install this `.tgz` file in another project to use it locally. Navigate to your project's directory and run:

    ```bash
    npm install path-to-your-tgz-file
    ```

Replace `path-to-your-tgz-file` with the actual path to the `.tgz` file.

⚠️ Remember to **rebuild and repack** your library every time you make changes to it. This way, you can test your Angular library locally without having to publish every version to npm. When you finally publish to NPM, in your consumer app uninstall the library and reinstall it as usual from NPM with `npm i library-name`.
