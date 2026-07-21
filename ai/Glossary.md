# AI Glossary

> This document contains important AI concepts that I am learning as part of my AI-assisted .NET Developer journey.

---

# Token

## Definition

A **Token** is the smallest unit of text that a Large Language Model (LLM) processes.

Unlike humans, an AI model does **not** read complete sentences. Instead, it breaks the input into smaller pieces called **tokens** and processes them one by one.

A token may represent:

- A complete word
- Part of a word
- A punctuation mark
- A number
- A special character
- A space or formatting symbol (depending on the tokenizer)

Different AI models may tokenize the same text differently.

---

## Simple Analogy

Imagine giving a child a box of LEGO bricks.

The child does not receive a finished castle.

Instead, the castle is built one LEGO brick at a time.

Similarly, an LLM does not process an entire paragraph at once.

It first breaks the paragraph into tokens and then reasons over those tokens.

---

## Why It Matters

Everything sent to an AI model is measured in tokens.

This includes:

- Your prompt
- Previous conversation history
- Source code
- Documentation
- PDFs
- SQL scripts
- Configuration files
- The AI's response

Every model has a maximum number of tokens it can process in a single request. This limit is known as the **Context Window**.

If the combined input and expected output exceed this limit:

- Older information may be discarded.
- Important context may be lost.
- Responses may become incomplete or inconsistent.

Understanding tokens helps developers write better prompts and manage large codebases more effectively.

---

## SQL Server Example

Consider the following SQL statement:

```sql
SELECT * FROM Users;
```

Humans see this as a single SQL statement.

An LLM processes it more like:

```
SELECT
*
FROM
Users
;
```

Each piece becomes one or more tokens.

---

## .NET Example

Suppose you ask ChatGPT to review the following files:

- Program.cs
- ProductController.cs
- ProductService.cs
- ProductRepository.cs
- appsettings.json
- SQL scripts
- README.md

Every file consumes tokens.

Large enterprise projects may contain hundreds of thousands of lines of code.

Since the Context Window is limited, developers often need to provide only the most relevant files.

---

## Practical Tips

- Keep prompts concise.
- Remove unnecessary text.
- Send only relevant source files.
- Split very large problems into multiple prompts.
- Be aware that both your input and the AI's output consume tokens.

---

## Interview Questions

### Q1. What is a Token?

A Token is the smallest unit of text processed by an LLM. Tokens may represent words, parts of words, punctuation, numbers, or symbols.

### Q2. Why are Tokens important?

Because every AI model has a limit on the number of tokens it can process in a single request.

### Q3. Does one word always equal one token?

No.

A word may become one token or multiple tokens depending on the tokenizer.

---

## Common Mistakes

❌ Thinking one word always equals one token.

❌ Forgetting that AI responses also consume tokens.

❌ Sending an entire source code repository when only a few files are required.

---

## Key Takeaways

- Tokens are the basic units processed by an LLM.
- Every prompt and response consumes tokens.
- Source code, documentation, and chat history all consume tokens.
- Token limits directly affect prompt design.

---

# Context Window

## Definition

The **Context Window** is the maximum amount of information an AI model can consider while generating a response.

Think of it as the model's **working memory** during a conversation.

Everything inside the Context Window is available for reasoning.

Everything outside it is forgotten unless provided again.

---

## Simple Analogy

Imagine explaining a software project to a new developer.

If they remember only the last five minutes of the discussion, you constantly repeat yourself.

If they remember the last two hours, they understand the complete picture.

A larger Context Window allows the AI to reason over more information.

---

## Why It Matters

A larger Context Window allows the model to understand:

- Large codebases
- Multiple APIs
- Long architecture documents
- Product requirements
- Design discussions
- Long conversations

Without sufficient context, the AI may:

- Forget earlier requirements.
- Produce inconsistent answers.
- Repeat previous explanations.
- Miss relationships between files.

---

## Real-world Example

Instead of sending only:

- ProductController.cs

You can send:

- ProductController.cs
- ProductService.cs
- ProductRepository.cs
- ProductDto.cs
- appsettings.json
- README.md
- SQL scripts

The AI can now understand how these files work together.

---

## .NET Example

Imagine asking ChatGPT:

> Review my ShoppingBuddy project.

If only one controller is provided, the AI cannot understand:

- Dependency Injection
- Repository Pattern
- Authentication
- Database schema
- Configuration

Providing the related files gives the model enough context to generate better recommendations.

---

## Practical Tips

- Include only relevant files.
- Keep conversations focused.
- Start a new chat for unrelated topics.
- Summarize long discussions when necessary.

---

## Interview Questions

### Q1. What is a Context Window?

The maximum amount of information an AI model can consider while generating a response.

### Q2. Why is a larger Context Window useful?

Because it enables the model to reason over larger codebases, longer documents, and more complex conversations.

### Q3. Can a larger Context Window replace good prompts?

No.

A larger Context Window helps, but clear prompts are still essential.

---

## Common Mistakes

❌ Assuming the AI remembers everything forever.

❌ Including unnecessary files.

❌ Mixing unrelated topics into one conversation.

---

## Key Takeaways

- Context Window is the model's working memory.
- Larger Context Windows improve reasoning.
- More context generally produces better answers.
- Good prompt design is still important.

---

# Prompt

## Definition

A **Prompt** is the instruction, question, or request given to an AI model.

The quality of the prompt directly influences the quality of the response.

A good prompt tells the model:

- What to do
- How to do it
- The desired output format
- Any constraints or context

---

## Simple Analogy

Imagine asking a taxi driver:

> Drive.

They don't know where you want to go.

Now imagine saying:

> Drive me to Pune Railway Station using the fastest route and avoid toll roads.

The second instruction is much clearer.

AI prompts work the same way.

---

## Poor Prompt

```text
Explain JWT.
```

---

## Better Prompt

```text
Explain JWT as if I am a Senior .NET Backend Engineer preparing for technical interviews.

Include:

- JWT structure
- Authentication flow
- ASP.NET Core implementation
- Security best practices
- Common interview questions
- Practical code examples
```

The second prompt provides context, audience, and expected output.

---

## .NET Example

Poor Prompt:

```text
Generate API code.
```

Better Prompt:

```text
Create a production-ready ASP.NET Core 8 Web API using:

- Clean Architecture
- Dependency Injection
- Repository Pattern
- Entity Framework Core
- SQL Server
- Swagger
- JWT Authentication
- Global Exception Handling
- Logging using Serilog
```

The second prompt produces significantly better results.

---

## Characteristics of a Good Prompt

A good prompt should be:

- Clear
- Specific
- Context-aware
- Goal-oriented
- Complete
- Easy to understand

---

## Practical Tips

- Describe the audience.
- Specify the programming language.
- Mention the framework version.
- Request the desired output format.
- Include constraints.
- Ask for explanations when learning.

---

## Interview Questions

### Q1. What is a Prompt?

A Prompt is the instruction or request provided to an AI model.

### Q2. Why is Prompt Engineering important?

Because better prompts generally produce better responses.

### Q3. What makes a good prompt?

A good prompt is clear, specific, contextual, and goal-oriented.

---

## Common Mistakes

❌ Writing vague prompts.

❌ Forgetting to provide context.

❌ Expecting the AI to guess requirements.

❌ Asking multiple unrelated questions in one prompt.

---

## Key Takeaways

- A Prompt is the primary way humans communicate with an AI model.
- Better prompts usually produce better responses.
- Context significantly improves AI output.
- Prompt Engineering is an essential skill for AI-assisted software development.

---

# Temperature

- Definition
- Simple Analogy
- Temperature Scale
- .NET Example
- OpenAI Example
- Interview Questions
- Common Mistakes
- Key Takeaways

---

# Top-P (Nucleus Sampling)

- Definition
- Temperature vs Top-P
- Real-world Example
- .NET Example
- Best Practices
- Interview Questions
- Common Mistakes
- Key Takeaways

---

# System Prompt

- Definition
- Analogy
- ChatGPT Example
- ShoppingBuddy Example
- Hidden Instructions
- Interview Questions
- Key Takeaways

---

# Summary

| Concept | Description |
|----------|-------------|
| Token | Smallest unit of text processed by an LLM |
| Context Window | Maximum amount of information an AI model can remember during one request |
| Prompt | The instruction given to an AI model |
| Temperature | Controls randomness and creativity |
| Top-P | Controls the candidate token pool |
| System Prompt | Hidden instructions defining the AI's role and behaviour |

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

### Q4. What is Temperature?

---

### Q5. Difference between Temperature and Top-P?

---

### Q6. What is a System Prompt ?

---

### Q7. Difference between System Prompt and User Prompt?

---

# My Notes

(To be completed during learning.)