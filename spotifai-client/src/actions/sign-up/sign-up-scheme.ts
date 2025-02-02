import { z } from "zod";

export const SignupFormSchema = z.object({
  email: z.string().email({ message: "Please enter a valid email." }).trim(),
  password: z
    .string()
    .min(5, { message: "Be at least 5 characters long" })
    .trim()
});

export type FormState =
  | {
      errors?: {
        name?: string[];
        email?: string[];
        password?: string[];
      };
      message?: string;
    }
  | undefined;
