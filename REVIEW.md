# Review

Took approx 4 hours

## Notes
- Writing to a JSON does not seem to be an ideal approach. SQLite or even inmemory database would work just as well.
- Struggled with TDD approach for the Repository so settled for simplistic coverage.
- Kept all logic within a single projects. Usual approach would be to split into 3 or more (typically Business and Data would be used along with entry point project)
- Would have spent less time if I spent more time confirming the requirements.
- Ended up skipping the collection file since the the services uses OpenAPI library to produce an interactive page for you to play with.

## Assumptions

- Player is the key and must be unique
- Score can be a negative integer
- All endpoints only accept `application/json`
- Not necessary to encrypt payloads
- Not necessary to compress payloads
- Simple loggin would suffice