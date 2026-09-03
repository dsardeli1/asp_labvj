import type { APIResponse, TestInfo } from '@playwright/test';

export function uniqueLabel(prefix: string, testInfo: TestInfo): string {
    return `${prefix}-${testInfo.project.name}-${testInfo.workerIndex}-${Date.now()}-${Math.random().toString(36).slice(2, 8)}`;
}

export async function readJson<T>(response: APIResponse): Promise<T> {
    return (await response.json()) as T;
}
