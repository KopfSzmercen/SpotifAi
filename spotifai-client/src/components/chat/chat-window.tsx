"use client";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { ScrollArea } from "@/components/ui/scroll-area";
import { useSession } from "next-auth/react";
import { useState } from "react";

type Message = {
  id: number;
  role: "user" | "ai";
  content: string;
};

interface RichTextProps {
  content: string;
}

const RichTextRenderer: React.FC<RichTextProps> = ({ content }) => {
  return (
    <div
      dangerouslySetInnerHTML={{
        __html: content.replaceAll(`"`, ``)
      }}
      className="prose max-w-none"
    />
  );
};

export default function ChatWindow() {
  const [isAiThinking, setIsAiThinking] = useState(false);
  const { data } = useSession();
  const [userInput, setUserInput] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);

  async function sentAiRequest() {
    setMessages((p) => [
      ...p,
      {
        id: messages.length + 1,
        role: "user",
        content: userInput
      }
    ]);

    setUserInput("");

    setIsAiThinking(true);
    const response = await fetch(
      process.env.NEXT_PUBLIC_BACKEND_ROOT_URL + "/scrapping/test",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          authorization: `Bearer ${data?.user.token}`
        },
        body: JSON.stringify({ command: userInput })
      }
    );

    if (!response.ok) throw new Error("Failed to fetch AI response");

    const aiResponse = await response.text();

    setMessages((p) => [
      ...p,
      {
        id: messages.length + 2,
        role: "ai",
        content: aiResponse
      }
    ]);

    setIsAiThinking(false);
  }

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-100">
      <Card className="w-full max-w-2xl">
        <CardHeader>
          <CardTitle className="flex justify-between">
            <p>SpotifAi Chat</p>
            <Button
              variant="outline"
              onClick={() => {
                setMessages([]);
              }}
            >
              New chat
            </Button>
          </CardTitle>
        </CardHeader>

        <CardContent>
          <ScrollArea className="h-[60vh] p-2">
            {messages.map((m) => (
              <div
                key={m.id}
                className={`mb-4 ${
                  m.role === "user" ? "text-right" : "text-left"
                }`}
              >
                <span
                  className={`inline-block p-2 rounded-lg ${
                    m.role === "user" ? "bg-primary text-white" : "text-black"
                  }`}
                >
                  {m.role === "user" ? (
                    m.content
                  ) : (
                    <RichTextRenderer content={m.content} />
                  )}
                </span>
              </div>
            ))}
            {isAiThinking && (
              <div className="text-left">
                <span className="inline-block p-2 rounded-lg bg-gray-200 text-black">
                  AI is thinking...
                </span>
              </div>
            )}
          </ScrollArea>
        </CardContent>
        <CardFooter>
          <form
            onSubmit={async (e) => {
              e.preventDefault();
              await sentAiRequest();
            }}
            className="flex w-full space-x-2"
          >
            <Input
              value={userInput}
              onChange={(e) => {
                setUserInput(e.target.value);
              }}
              placeholder="Type your message..."
              className="flex-grow"
              disabled={isAiThinking}
            />
            <Button type="submit" disabled={isAiThinking || !userInput}>
              Send
            </Button>
          </form>
        </CardFooter>
      </Card>
    </div>
  );
}
