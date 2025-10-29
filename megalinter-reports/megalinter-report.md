## ✅⚠️[MegaLinter](https://megalinter.io/9.0.1) analysis: Success with warnings



| Descriptor  |                                               Linter                                                |Files|Fixed|Errors|Warnings|Elapsed time|
|-------------|-----------------------------------------------------------------------------------------------------|----:|----:|-----:|-------:|-----------:|
|✅ ACTION    |[actionlint](https://megalinter.io/9.0.1/descriptors/action_actionlint)                              |    1|     |     0|       0|       0.24s|
|✅ CSHARP    |[csharpier](https://megalinter.io/9.0.1/descriptors/csharp_csharpier)                                |    6|    6|     0|       0|       2.62s|
|✅ JSON      |[jsonlint](https://megalinter.io/9.0.1/descriptors/json_jsonlint)                                    |    2|     |     0|       0|       0.18s|
|✅ JSON      |[npm-package-json-lint](https://megalinter.io/9.0.1/descriptors/json_npm_package_json_lint)          |  yes|     |    no|      no|        0.4s|
|✅ JSON      |[prettier](https://megalinter.io/9.0.1/descriptors/json_prettier)                                    |    2|    0|     0|       0|        0.3s|
|✅ JSON      |[v8r](https://megalinter.io/9.0.1/descriptors/json_v8r)                                              |    2|     |     0|       0|       3.53s|
|⚠️ MARKDOWN  |[markdownlint](https://megalinter.io/9.0.1/descriptors/markdown_markdownlint)                        |    1|    0|     1|       0|       0.63s|
|✅ MARKDOWN  |[markdown-table-formatter](https://megalinter.io/9.0.1/descriptors/markdown_markdown_table_formatter)|    1|    0|     0|       0|       0.27s|
|✅ REPOSITORY|[gitleaks](https://megalinter.io/9.0.1/descriptors/repository_gitleaks)                              |  yes|     |    no|      no|       0.31s|
|✅ REPOSITORY|[git_diff](https://megalinter.io/9.0.1/descriptors/repository_git_diff)                              |  yes|     |    no|      no|       0.01s|
|✅ REPOSITORY|[grype](https://megalinter.io/9.0.1/descriptors/repository_grype)                                    |  yes|     |    no|      no|      25.16s|
|✅ REPOSITORY|[secretlint](https://megalinter.io/9.0.1/descriptors/repository_secretlint)                          |  yes|     |    no|      no|       0.56s|
|✅ REPOSITORY|[syft](https://megalinter.io/9.0.1/descriptors/repository_syft)                                      |  yes|     |    no|      no|       1.03s|
|✅ REPOSITORY|[trivy-sbom](https://megalinter.io/9.0.1/descriptors/repository_trivy_sbom)                          |  yes|     |    no|      no|        0.1s|
|✅ REPOSITORY|[trufflehog](https://megalinter.io/9.0.1/descriptors/repository_trufflehog)                          |  yes|     |    no|      no|       2.17s|
|✅ YAML      |[prettier](https://megalinter.io/9.0.1/descriptors/yaml_prettier)                                    |    2|    0|     0|       0|       0.35s|
|✅ YAML      |[v8r](https://megalinter.io/9.0.1/descriptors/yaml_v8r)                                              |    2|     |     0|       0|       3.82s|

## Detailed Issues

<details>
<summary>⚠️ MARKDOWN / markdownlint - 1 error</summary>

```
sqlProject/migration-tools/sql-to-graph/README.md:102 MD040/fenced-code-language Fenced code blocks should have a language specified [Context: "```"]
```

</details>

See detailed reports in MegaLinter artifacts
_Set `VALIDATE_ALL_CODEBASE: true` in mega-linter.yml to validate all sources, not only the diff_

[![MegaLinter is graciously provided by OX Security](https://raw.githubusercontent.com/oxsecurity/megalinter/main/docs/assets/images/ox-banner.png)](https://www.ox.security/?ref=megalinter)