# AI Glossary

> This document contains important AI concepts that I am learning as part of my AI-assisted .NET Developer journey.

---

# Token

## Definition

A **Token** is the smallest unit of text that a Large Language Model (LLM) processes. A token may represent:

- A complete word
- Part of a word
- A punctuation mark
- A special character
- A number

Unlike humans, an LLM does not read complete sentences. Instead, it processes input one token at a time.

---

## SQL Server Example

Consider the following SQL statement:

```sql
SELECT * FROM Users;
```

As humans, we think of this as:

- One SQL statement

An LLM processes it more like:

```
SELECT
*
FROM
Users
;
```

Each of these pieces is treated as one or more **tokens**.

---

## Why Should a .NET Developer Care?

Suppose you paste the following into ChatGPT:

- Program.cs
- Startup.cs
- ProductController.cs
- ProductRepository.cs
- SQL Scripts
- Configuration Files

All of this consumes tokens.

Every AI model has a maximum number of tokens that it can process at one time. This limit is called the **Context Window**.

If your input exceeds that limit:

- Earlier information may be ignored.
- The model may lose important context.
- Responses may become incomplete or inaccurate.

Understanding tokens helps developers write better prompts and manage large codebases more effectively.

---

## Key Points

- Every prompt consumes tokens.
- Every AI response consumes tokens.
- Source code also consumes tokens.
- Large projects require careful token management.

---

# Context Window

## Definition

The **Context Window** is the maximum amount of information that an AI model can remember and use during a single conversation or request.

Think of it as the AI model's short-term working memory.

---

## Real-World Analogy

Imagine explaining a software project to a new developer.

If they remember only the last five minutes of the discussion, you constantly need to repeat yourself.

If they remember the last two hours, they understand the complete picture.

Similarly, a larger Context Window allows an AI model to reason over much larger amounts of information.

---

## Why Is It Important?

A larger Context Window enables the AI model to process:

- Large source code repositories
- Long technical documents
- Multiple API files
- Architecture documents
- Long conversations
- Requirement specifications

---

## Example

Instead of sending:

- ProductController.cs

You can send:

- ProductController.cs
- ProductService.cs
- ProductRepository.cs
- appsettings.json
- SQL Script
- README.md

The AI can understand how all these files relate to one another.

---

## Key Points

- Larger Context Window = More information remembered.
- Better context usually produces better answers.
- Very large projects may still need to be broken into smaller parts.

---

# Prompt

## Definition

A **Prompt** is the instruction or question given to an AI model.

The quality of the prompt directly affects the quality of the response.

---

## Poor Prompt

```
Explain JWT.
```

---

## Better Prompt

```
Explain JWT as if I am a Senior .NET Backend Engineer preparing for technical interviews.

Include:

- JWT structure
- Authentication flow
- Advantages
- Security considerations
- Common interview questions
- Practical ASP.NET Core examples
```

The second prompt provides much more context, allowing the AI to generate a more useful response.

---

## Characteristics of a Good Prompt

A good prompt should be:

- Clear
- Specific
- Context-aware
- Goal-oriented
- Easy to understand

---

## Practical Tip

Instead of asking:

```
Write code.
```

Ask:

```
Create a production-ready ASP.NET Core 8 Web API following Clean Architecture, using Dependency Injection, Repository Pattern, Entity Framework Core, SQL Server, Swagger, JWT Authentication, and proper exception handling.
```

The more useful context you provide, the better the AI's response is likely to be.

---

# Summary

| Concept | Description |
|----------|-------------|
| Token | Smallest unit of text processed by an LLM |
| Context Window | Maximum amount of information an AI model can remember during one request |
| Prompt | The instruction given to an AI model |

---

# Interview Questions

### Q1. What is a Token?

A Token is the smallest unit of text processed by an LLM. Tokens can represent words, parts of words, punctuation, or symbols.

---

### Q2. What is a Context Window?

A Context Window is the maximum amount of information an AI model can consider while generating a response.

---

### Q3. Why is Prompt Engineering important?

Because the quality, clarity, and context of the prompt directly influence the quality of the AI-generated response.

---

# My Notes

(To be completed during learning.)