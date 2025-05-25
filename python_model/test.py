import os
import asyncio
from translate import translate_text_async


async def function():
   return await translate_text_async("Hello world what is that","en","pl")

text = asyncio.run(function())
print(text)