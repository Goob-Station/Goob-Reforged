# Common Resources

Common Resources are resources that are widely shared among forks and
tend to be heavily modified by the main fork either way.

An example of this is `Species`.

Species tend to have their own flavour or implementation in forks.
Thus their `.yml` is heavily modfied, and directoties are all over the place.

Certain issues with including each species in the resource folder from the originating fork include:

### Equal Species with differing origins.
- Resomi
- Vulpkanin

Resomi are a relatively popular species, originally PR'd in `Wizden` but not implemented.

Historically, `Floofstation` Adopted this species, which `Goobstation` ported,
however other forks like `Starlight` Implemented their own version from `Wizden`.

This lead to effectively two different directories for the same species.
Which makes porting content unwieldy and makes it unclear
as to what content originated from where either way

Same issue exists with `Vulpkanin`, originally being a species implemented in `DeltaV`,
now being also being a species in `Wizden`.

Thus, generally, this resource folder exists not to adopt ownership of files or content,
but to simplify the porting and implementation of each.

- [ ] todo more examples of usages for common resources.

## Policy

Resources in `CommonResources` Should be resources that may be used across forks.

Code for features should still remain in the respective fork direcotory,
unless its functionality specifically made to be shared among them.

### Origin marking:

Reforged is a project about remaking the `Goobstation` Codebase.
During It's development numerous resources and features have been modified in such an extensive manner,
barely anything remains from their original port or otherwise implementation.

Thus, we can assume that `any` common resource will follow a similar path.

- [ ] todo, think of better way of marking files.

The top of each file should have a comment describing the origin or precedence of the file like such,
listing modifications made by any other forks to this content.

```
# The following content originally ported from:
# - Goobstation

# With current Modifying changes from:
# - <ForkName>
# - <ForkNameTwo>

- type: entity
  id: someEntity
  name: exampleEntity
  components:
  - type: OriginalComponents
  (...)
  # <Forkname> Edit Start
  - type: ExtraComponents
  (...)
  # <ForkName> Edit End
```

- [ ] todo, write up upstreaming documentation and make yaml modification more clear.
