"use client";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { signup } from "@/actions/sign-up/sign-up";
import { useActionState } from "react";
import { toast } from "sonner";
import { useRouter } from "next/navigation";

export function RegisterForm({
  className,
  ...props
}: React.ComponentPropsWithoutRef<"div">) {
  const [state, action, pending] = useActionState(signup, undefined);
  const router = useRouter();

  if (state?.success) {
    toast.success("Registration successful.");
    router.push("/login");
  }

  return (
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle className="text-2xl">Register</CardTitle>
          <CardDescription>Enter your email below to register</CardDescription>
        </CardHeader>
        <CardContent>
          <form action={action}>
            <div className="flex flex-col gap-6">
              <div className="grid gap-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  type="email"
                  name="email"
                  placeholder="m@example.com"
                  required
                />
                {state?.errors?.email && <p>{state.errors.email}</p>}
              </div>
              <div className="grid gap-2">
                <div className="flex items-center">
                  <Label htmlFor="password">Password</Label>
                </div>
                <Input id="password" type="password" name="password" required />
                {state?.errors?.password && (
                  <p className="text-error">
                    {state.errors.password.join(".")}
                  </p>
                )}
              </div>
              {state?.apiError && (
                <p className="text-error">{state.apiError}</p>
              )}
              <Button type="submit" className="w-full" disabled={pending}>
                Register
              </Button>
            </div>
            <div className="mt-4 text-center text-sm">
              Have an account?{" "}
              <a href="/login" className="underline underline-offset-4">
                Sign in
              </a>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  );
}
