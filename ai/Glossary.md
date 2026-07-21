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

## Definition

**Temperature** is a parameter that controls how **creative, random, or deterministic** an AI model's response should be.

It does **not** control the intelligence, knowledge, or accuracy of the model.

Think of Temperature as a **Creativity Knob**.

- Lower Temperature → More predictable responses
- Higher Temperature → More creative responses

---

## Simple Analogy

Imagine asking 100 students:

> What is 2 + 2?

Almost everyone answers:

```
4
```

Now ask:

> Write a story about a dragon.

Some students write similar stories.

Some create completely different worlds.

Temperature controls how adventurous the AI becomes when choosing its next words.

---

## Why It Matters

Choosing the right Temperature improves the quality of AI responses.

Examples:

Low Temperature is useful for:

- Code generation
- SQL queries
- JSON
- APIs
- Mathematics
- Technical documentation

Higher Temperature is useful for:

- Brainstorming
- Story writing
- Marketing
- Product ideas
- Creative writing

---

## Temperature Scale

### Temperature = 0

Very deterministic

Almost identical answers every time.

Best for:

- Coding
- SQL
- Mathematics
- APIs
- JSON generation
- Unit tests

--------------------------------------

### Temperature = 0.2

Very low creativity

Suitable for:

- Business emails
- Documentation
- Architecture discussions
- Technical writing

--------------------------------------

### Temperature = 0.5

Balanced

Suitable for:

- Learning
- Tutorials
- Chat assistants
- Summaries

--------------------------------------

### Temperature = 0.7

Creative

Suitable for:

- Brainstorming
- Product names
- UI ideas
- Design discussions

--------------------------------------

### Temperature = 1.0+

Highly creative

Suitable for:

- Stories
- Poetry
- Role-playing
- Idea generation

May become less consistent.

---

## Real-world Example

Prompt:

```
Write a C# method to reverse a string.
```

Temperature = 0

The generated code will usually be nearly identical every time.

--------------------------------------

Prompt:

```
Suggest five AI startup ideas.
```

Temperature = 1

Each execution may produce different ideas.

---

## .NET Example

Suppose your ASP.NET Core API exposes:

```
POST /api/chat
```

Recommended values:

Coding Assistant

```
Temperature = 0.1
```

Interview Coach

```
Temperature = 0.3
```

Architecture Assistant

```
Temperature = 0.5
```

Marketing Assistant

```
Temperature = 0.8
```

---

## OpenAI Example

```csharp
temperature = 0.2
```

Produces deterministic technical responses.

---

## Interview Questions

### Q1. What does Temperature control?

Temperature controls the randomness and creativity of AI responses.

---

### Q2. Does Temperature increase intelligence?

No.

It only changes how the model selects words.

---

### Q3. Which Temperature is preferred for code generation?

Usually between **0 and 0.2**.

---

## Common Mistakes

❌ Thinking higher Temperature means smarter AI.

❌ Using Temperature 1.0 for SQL generation.

❌ Using Temperature 0 for brainstorming.

---

## Key Takeaways

- Temperature controls creativity.
- Lower values produce more predictable output.
- Higher values produce more diverse output.
- Coding assistants usually use low Temperature.

---

# Top-P (Nucleus Sampling)

## Definition

**Top-P** (Top Probability), also called **Nucleus Sampling**, controls **how many candidate words the AI considers before selecting the next token**.

Unlike Temperature, which changes randomness, Top-P changes the **size of the candidate pool**.

---

## Simple Analogy

Imagine a classroom.

The teacher asks:

> Name a programming language.

If Top-P is very low,

the AI considers only the most likely answers:

- C#
- Java
- Python

If Top-P is high,

the AI may also consider:

- Kotlin
- Rust
- Elixir
- Zig
- Haskell

A larger candidate pool increases diversity.

---

## Why It Matters

Top-P controls how broad the AI's vocabulary selection becomes.

Lower Top-P

- Safer
- More predictable
- Less variety

Higher Top-P

- More variety
- More creativity
- Slightly higher chance of unusual outputs

---

## Temperature vs Top-P

| Temperature | Top-P |
|--------------|-------|
| Controls randomness | Controls candidate pool |
| Creativity knob | Vocabulary selection |
| Changes probability distribution | Filters possible next tokens |
| Most commonly adjusted | Usually left at default |

---

## Real-world Example

Prompt

```
Suggest names for an AI startup.
```

Top-P = 0.3

Mostly common names.

--------------------------------------

Top-P = 0.95

Much more diverse suggestions.

---

## .NET Example

A Banking API assistant

```
Temperature = 0.2

Top-P = 0.9
```

Produces deterministic answers while still considering high-probability vocabulary.

---

## Best Practices

Most applications change either:

- Temperature

OR

- Top-P

Not both.

OpenAI also recommends adjusting one before experimenting with the other.

---

## Interview Questions

### Q1. What is Top-P?

Top-P controls the size of the candidate token pool.

---

### Q2. Difference between Temperature and Top-P?

Temperature controls randomness.

Top-P controls which candidate tokens are considered.

---

### Q3. Should both always be changed together?

No.

Usually only one parameter is adjusted.

---

## Common Mistakes

❌ Confusing Top-P with Temperature.

❌ Believing Top-P changes intelligence.

❌ Setting both parameters randomly.

---

## Key Takeaways

- Top-P controls candidate selection.
- Temperature controls randomness.
- Most applications modify only one of them.
- Coding assistants often use a low Temperature while keeping Top-P near its default.

---

# System Prompt

## Definition

A **System Prompt** is a hidden instruction provided to the AI model by the application developer before the user starts interacting with it.

It defines the AI's:

- Role
- Behaviour
- Tone
- Goals
- Restrictions
- Response style

Users normally cannot see the System Prompt.

---

## Simple Analogy

Imagine joining a new company.

Before speaking with customers, your manager gives you instructions:

- Be polite.
- Never disclose confidential information.
- Help customers solve problems.
- Escalate billing issues.
- Follow company policies.

Those manager instructions are like the **System Prompt**.

Customer questions are like **User Prompts**.

---

## Why It Matters

The System Prompt provides consistent behaviour across conversations.

Without it, the AI has no predefined role.

With it, the AI knows whether it should behave as:

- Software Architect
- Coding Assistant
- Medical Assistant
- Travel Planner
- Customer Support Agent

---

## ShoppingBuddy Example

Example System Prompt

```text
You are ShoppingBuddy AI Assistant.

Your goal is to help users find suitable shopping buddies.

Rules:

- Prefer buddies who are currently online.
- If none are online, suggest recently active buddies.
- Match users by location first.
- Then by interests.
- Then by age group.
- Show buddy ratings if available.
- Never recommend blocked or inactive users.
- If no suitable buddy exists, politely explain why and suggest increasing the search radius.
```

---

## ChatGPT Example

A simplified System Prompt might be:

```text
You are a helpful AI assistant.

Provide accurate, clear, and safe answers.

If you do not know something, say so instead of inventing information.
```

---

## .NET Example

Imagine building an ASP.NET Core AI application.

Your backend sends this System Prompt:

```text
You are an Interview Coach.

Help Senior .NET Developers prepare for technical interviews.

Explain concepts clearly.

Provide production-quality C# examples.

End every lesson with three interview questions.
```

Every user now receives responses following those rules.

---

## Interview Questions

### Q1. What is a System Prompt?

A hidden instruction that defines the AI's behaviour before the user begins interacting with it.

---

### Q2. Who usually writes the System Prompt?

The application developer or AI application designer.

---

### Q3. Can users normally see the System Prompt?

No.

It is typically hidden from end users.

---

## Common Mistakes

❌ Assuming the System Prompt is the same as a User Prompt.

❌ Believing users usually write the System Prompt.

❌ Thinking the System Prompt guarantees perfect behaviour.

---

## Key Takeaways

- The System Prompt defines the AI's role.
- It is written by the application developer.
- It is usually hidden from users.
- It helps produce consistent responses across conversations.
- Enterprise AI applications rely heavily on well-designed System Prompts.

---

# User Prompt

## Definition

A **User Prompt** is the instruction, question, or request entered by the end user during a conversation with an AI model.

Unlike the **System Prompt**, which is written by the application developer, the User Prompt is written by the person interacting with the AI.

The User Prompt tells the AI what the user wants to accomplish.

---

## Simple Analogy

Imagine visiting a restaurant.

The restaurant has:

- Company policies
- Chef guidelines
- Kitchen rules

These are similar to the **System Prompt**.

Now you tell the waiter:

> I would like a Masala Dosa without butter.

That request is the **User Prompt**.

---

## Why It Matters

Every conversation with an AI starts with one or more User Prompts.

The quality of the User Prompt directly affects:

- Accuracy
- Relevance
- Completeness
- Response quality

Good User Prompts help the AI understand:

- The objective
- The required output
- The audience
- Any constraints

---

## ShoppingBuddy Example

User enters:

```text
Find shopping buddies near Kothrud who are interested in grocery shopping this evening.
```

The AI now understands:

- Location → Kothrud
- Activity → Grocery shopping
- Time → Evening

---

## .NET Example

Poor User Prompt

```text
Generate API.
```

Better User Prompt

```text
Create a production-ready ASP.NET Core 8 Web API using Clean Architecture, Entity Framework Core, SQL Server, JWT Authentication, Swagger, Dependency Injection and Global Exception Handling.
```

The second prompt provides significantly more context.

---

## OpenAI Example

```json
{
  "role": "user",
  "content": "Explain Dependency Injection with ASP.NET Core examples."
}
```

---

## Interview Questions

### Q1. What is a User Prompt?

A User Prompt is the instruction or request provided by the end user to an AI model.

---

### Q2. Who writes the User Prompt?

The end user.

---

### Q3. How does a User Prompt differ from a System Prompt?

A User Prompt contains the user's request.

A System Prompt defines the AI's behaviour.

---

## Common Mistakes

❌ Writing vague prompts.

❌ Expecting the AI to guess missing information.

❌ Mixing multiple unrelated questions in one prompt.

---

## Key Takeaways

- Users write User Prompts.
- Better prompts produce better responses.
- User Prompts work together with the System Prompt.
- Every AI conversation contains one or more User Prompts.

---

# Assistant Message

## Definition

An **Assistant Message** is the response generated by the AI model after processing the available conversation.

It is created using:

- System Prompt
- User Prompt
- Conversation History
- Context Window
- Model reasoning (Inference)

The Assistant Message is what the user finally sees.

---

## Simple Analogy

Imagine a customer support center.

Manager Instructions

↓

Customer Question

↓

Support Executive Response

The support executive's reply is similar to the **Assistant Message**.

---

## Why It Matters

Assistant Messages should be:

- Accurate
- Helpful
- Relevant
- Clear
- Context-aware

Enterprise AI applications often define formatting rules for Assistant Messages.

For example:

- Always answer in Markdown.
- Always produce JSON.
- Always end with interview questions.
- Always explain step by step.

---

## ShoppingBuddy Example

User Prompt

```text
Find shopping buddies near Baner.
```

Assistant Message

```text
I found four available shopping buddies.

1. Rahul
Online
Interested in Grocery Shopping

2. Sneha
Recently Active
Interested in Electronics

Would you like to filter by age or interests?
```

---

## .NET Example

User Prompt

```text
Generate an ASP.NET Core Web API.
```

Assistant Message

The AI generates:

- Controller
- Service
- Repository
- Entity
- DTO
- Dependency Injection
- Swagger configuration
- Program.cs updates

---

## OpenAI Example

```json
{
  "role": "assistant",
  "content": "Dependency Injection is a design pattern..."
}
```

---

## Interview Questions

### Q1. What is an Assistant Message?

The response generated by the AI model.

---

### Q2. What information is used to generate an Assistant Message?

- System Prompt
- User Prompt
- Conversation History
- Context Window

---

### Q3. Can two Assistant Messages be different for the same prompt?

Yes.

Depending on Temperature, Top-P and model behaviour.

---

## Common Mistakes

❌ Assuming the Assistant only reads the latest User Prompt.

❌ Ignoring previous conversation history.

❌ Forgetting that Assistant Messages also consume tokens.

---

## Key Takeaways

- Assistant Messages are generated by the AI.
- They depend on the complete conversation.
- They are influenced by Temperature and Top-P.
- Every response becomes part of the future conversation history.

---

# Conversation History

## Definition

**Conversation History** is the collection of previous messages exchanged between the user and the AI during a conversation.

It includes:

- System Prompt
- User Prompts
- Assistant Messages

The AI uses Conversation History to maintain continuity and provide context-aware responses.

---

## Simple Analogy

Imagine speaking with a colleague throughout the day.

You do not repeat your introduction every five minutes.

Both of you remember the earlier discussion.

Conversation History works the same way.

---

## Why It Matters

Without Conversation History, the AI would treat every prompt as a completely new conversation.

Conversation History allows the AI to:

- Remember previous questions
- Avoid repeating information
- Continue unfinished discussions
- Maintain context
- Answer follow-up questions naturally

---

## ShoppingBuddy Example

User

```text
Find shopping buddies near Kothrud.
```

Assistant

```text
Found four buddies.
```

User

```text
Show only those interested in grocery shopping.
```

The second prompt works because the AI remembers the previous response.

---

## .NET Example

User

```text
Generate ProductController.
```

Assistant generates the controller.

User

```text
Now generate the Service.
```

The AI understands that "Service" refers to the Product API because of the Conversation History.

---

## OpenAI Example

Typical API request

```text
System

↓

User

↓

Assistant

↓

User

↓

Assistant

↓

User
```

Every message is sent together in the request until the Context Window is exceeded.

---

## Relationship with Context Window

Conversation History consumes tokens.

As the conversation grows longer:

- More tokens are consumed.
- Older messages may eventually fall outside the Context Window.
- The AI may stop considering very old messages.

This is why long conversations sometimes lose earlier context.

---

## Interview Questions

### Q1. What is Conversation History?

The collection of previous messages exchanged between the user and the AI.

---

### Q2. Why is Conversation History important?

It enables context-aware conversations and follow-up questions.

---

### Q3. Does Conversation History consume tokens?

Yes.

Every previous message contributes to token usage.

---

## Common Mistakes

❌ Assuming the AI remembers every conversation forever.

❌ Forgetting that Conversation History consumes the Context Window.

❌ Mixing unrelated topics into one very long conversation.

---

## Key Takeaways

- Conversation History enables natural conversations.
- Previous messages improve context.
- Long conversations consume more tokens.
- Older messages may eventually fall outside the Context Window.

---

# Context Management

## Definition

**Context Management** is the process of selecting, organizing, and maintaining the information that an AI model uses to generate accurate and relevant responses.

Since every AI model has a limited **Context Window**, applications must carefully decide what information should be sent with each request.

Good Context Management ensures that the AI receives the right information at the right time.

---

## Simple Analogy

Imagine preparing a colleague for an important customer meeting.

Instead of giving them every email ever exchanged, you provide:

- The customer's requirements
- Previous meeting notes
- The latest design
- Current action items

This allows your colleague to answer confidently without being overwhelmed.

AI applications work the same way.

---

## Why It Matters

Poor Context Management can cause the AI to:

- Forget earlier requirements
- Give inconsistent answers
- Repeat previous information
- Produce irrelevant responses

Good Context Management helps the AI:

- Understand the user's intent
- Remember important details
- Ignore unnecessary information
- Produce more accurate responses

---

## What Typically Forms the Context?

A modern AI application may include:

- System Prompt
- User Prompt
- Previous Assistant Messages
- Conversation History
- Retrieved documents (RAG)
- Source code
- Database results
- User profile
- Application state

The application decides what to include before sending the request to the model.

---

## ShoppingBuddy Example

Suppose the user asks:

```text
Find grocery shopping buddies near Baner.
```

The application may send:

- System Prompt
- User Prompt
- User's location
- Available buddies
- Online status
- User preferences

It does **not** send the entire database.

Only relevant information is included.

---

## .NET Example

Imagine your AI assistant helps developers understand the ShoppingBuddy project.

Instead of sending the entire repository, your ASP.NET Core application sends only:

- ProductController.cs
- ProductService.cs
- ProductRepository.cs
- DTOs
- appsettings.json

This reduces token usage while improving response quality.

---

## Best Practices

- Send only relevant information.
- Remove duplicate content.
- Summarize long conversations.
- Retrieve documents only when needed (RAG).
- Keep prompts focused.

---

## Interview Questions

### Q1. What is Context Management?

Context Management is the process of selecting and organizing the information provided to an AI model.

---

### Q2. Why is Context Management important?

Because AI models have limited Context Windows and cannot process unlimited information.

---

### Q3. Who performs Context Management?

Usually the application developer or AI framework before calling the LLM.

---

## Common Mistakes

❌ Sending the entire database.

❌ Sending every source code file.

❌ Including unrelated conversation history.

❌ Ignoring token limits.

---

## Key Takeaways

- Context Management improves response quality.
- Only relevant information should be included.
- Good Context Management reduces token usage.
- RAG is commonly used as part of Context Management.

---

# Token Usage

## Definition

**Token Usage** refers to the number of tokens consumed by an AI request and its corresponding response.

Every interaction with an LLM consumes tokens.

Token usage directly affects:

- Cost
- Performance
- Context Window usage
- Response length

---

## What Consumes Tokens?

Tokens are consumed by:

- System Prompt
- User Prompt
- Assistant Messages
- Conversation History
- Retrieved documents
- Source code
- The AI's generated response

Everything sent to or returned from the model contributes to token usage.

---

## Why It Matters

Understanding token usage helps developers:

- Reduce API costs
- Improve performance
- Stay within Context Window limits
- Build scalable AI applications

---

## ShoppingBuddy Example

Suppose a user asks:

```text
Find shopping buddies near Pune.
```

The application sends:

- System Prompt
- User Prompt
- Five buddy profiles

If instead it sends all 50,000 users in the database, token usage increases dramatically.

---

## .NET Example

Instead of sending:

```
Entire Solution
```

Send only:

- ProductController.cs
- ProductService.cs
- ProductRepository.cs

This reduces token consumption while preserving useful context.

---

## Cost Example

Imagine:

Input

```
1500 tokens
```

Output

```
500 tokens
```

Total token usage

```
2000 tokens
```

Most commercial AI providers charge based on:

- Input tokens
- Output tokens

---

## Best Practices

- Keep prompts concise.
- Send only relevant files.
- Summarize long conversations.
- Remove duplicate information.
- Cache repeated content when possible.

---

## Interview Questions

### Q1. What consumes tokens?

Both input and output consume tokens.

---

### Q2. Why is token usage important?

Because it affects cost, performance, and Context Window usage.

---

### Q3. Do AI responses consume tokens?

Yes.

Generated responses also consume tokens.

---

## Common Mistakes

❌ Assuming only prompts consume tokens.

❌ Sending unnecessary documents.

❌ Ignoring API costs.

---

## Key Takeaways

- Every request consumes tokens.
- Every response consumes tokens.
- Lower token usage reduces cost.
- Efficient prompts improve scalability.

---

# Inference

## Definition

**Inference** is the process of generating a response using a trained AI model.

During inference, the model **does not learn anything new**.

Instead, it uses its existing knowledge, together with the provided context, to predict the most appropriate next token until the response is complete.

Inference is what happens every time you press **Send** in ChatGPT or any AI application.

---

## Simple Analogy

Imagine an experienced software architect.

When asked:

> How should I design this API?

The architect does not go back to university and study again.

Instead, they use their existing knowledge and experience to answer.

LLMs work in a similar way during inference.

---

## What Happens During Inference?

A simplified flow is:

```text
User Prompt
        │
        ▼
System Prompt
        │
        ▼
Conversation History
        │
        ▼
Context Management
        │
        ▼
LLM
        │
Predict Next Token
        │
Predict Next Token
        │
Predict Next Token
        │
...
        │
        ▼
Assistant Message
```

The model predicts one token at a time until the response is complete.

---

## ShoppingBuddy Example

User:

```text
Find shopping buddies near Kothrud.
```

The application:

- Builds the context
- Sends the request to the LLM

The LLM performs inference and returns:

```text
I found four shopping buddies near Kothrud who are currently online...
```

---

## .NET Example

An ASP.NET Core API receives:

```http
POST /api/chat
```

The backend:

- Creates the System Prompt
- Adds the User Prompt
- Adds conversation history
- Calls Azure OpenAI

Azure OpenAI performs inference and returns the Assistant Message.

---

## Important Difference

### Training

The AI learns from enormous datasets.

This happens once during model development.

### Inference

The trained model answers user questions.

No new learning takes place.

---

## Interview Questions

### Q1. What is inference?

Inference is the process of generating responses using a trained AI model.

---

### Q2. Does the model learn during inference?

No.

It only uses previously learned knowledge together with the current context.

---

### Q3. What does the model predict during inference?

It predicts the most probable next token repeatedly until the response is complete.

---

## Common Mistakes

❌ Thinking the AI learns from every conversation.

❌ Confusing training with inference.

❌ Assuming the model searches the internet before every response.

---

## Key Takeaways

- Inference generates responses.
- The model predicts one token at a time.
- No learning occurs during inference.
- Every AI chat application performs inference whenever the user sends a request.

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